using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

class AutoServiceGame
{
    private int money;
    private Dictionary<string, int> warehouse;
    private List<PurchaseOrder> orders;
    private Random random;
    private string connectionString;
    private int gameId;

    public AutoServiceGame(int startMoney, string connectionString)
    {
        this.connectionString = connectionString;
        money = startMoney;
        warehouse = new Dictionary<string, int>();
        orders = new List<PurchaseOrder>();
        random = new Random();
        InitializeGame();
    }

    private void InitializeGame()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        // Создаем игровую сессию
        var cmd = new SqlCommand("INSERT INTO GameSessions (StartMoney, CurrentMoney) OUTPUT INSERTED.Id VALUES (@money, @money)", connection);
        cmd.Parameters.AddWithValue("@money", money);
        gameId = (int)cmd.ExecuteScalar();

        // Загружаем начальные детали
        cmd = new SqlCommand("SELECT Name, InitialQuantity FROM Parts WHERE IsActive = 1 AND InitialQuantity > 0", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string name = reader["Name"].ToString();
            int quantity = (int)reader["InitialQuantity"];
            warehouse[name] = quantity;
            SaveInventory(name, quantity);
        }
    }

    public void RunGame()
    {
        Console.WriteLine("=== АВТОСЕРВИС ===\nНачальный баланс: " + money + " руб.\nНажмите любую клавишу...");
        Console.ReadKey();

        for (int client = 1; ; client++)
        {
            Console.Clear();
            Console.WriteLine($"КЛИЕНТ №{client}");

            ProcessDeliveries();
            ShowStatus();

            string brokenPart = GetRandomPart();
            int repairCost = GetPartPrice(brokenPart) + random.Next(200, 800);
            Console.WriteLine($"\nПоломка: {brokenPart}\nСтоимость ремонта: {repairCost} руб.");

            Console.WriteLine("\n1 - Взять заказ\n2 - Отказать (штраф 300)\n3 - Закупить\n4 - Выйти");
            switch (Console.ReadLine())
            {
                case "1": AcceptOrder(brokenPart, repairCost, client); break;
                case "2": RefuseOrder(client); break;
                case "3": ShowPurchaseMenu(); break;
                case "4": SaveGame(); Console.WriteLine($"Игра окончена! Баланс: {money} руб."); return;
                default: Console.WriteLine("Неверный выбор!"); Console.ReadKey(); client--; continue;
            }
            Console.WriteLine("Нажмите любую клавишу..."); Console.ReadKey();
        }
    }

    private void ProcessDeliveries()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        // Доставляем готовые заказы
        var cmd = new SqlCommand("SELECT * FROM PurchaseOrders WHERE GameId = @id AND DeliveryCounter <= 0", connection);
        cmd.Parameters.AddWithValue("@id", gameId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string part = reader["PartName"].ToString();
            int quantity = (int)reader["Quantity"];
            warehouse[part] = warehouse.GetValueOrDefault(part) + quantity;
            Console.WriteLine($"✓ Доставлены {quantity} {part}");
            SaveInventory(part, warehouse[part]);
            new SqlCommand("DELETE FROM PurchaseOrders WHERE Id = " + reader["Id"], connection).ExecuteNonQuery();
        }
        reader.Close();

        // Уменьшаем счетчики
        new SqlCommand("UPDATE PurchaseOrders SET DeliveryCounter = DeliveryCounter - 1 WHERE GameId = @id", connection)
            .Parameters.AddWithValue("@id", gameId).ExecuteNonQuery();

        UpdateOrders();
    }

    private void ShowStatus()
    {
        Console.WriteLine($"\nБаланс: {money} руб.\nСклад:");
        foreach (var part in warehouse) Console.WriteLine($"  {part.Key}: {part.Value} шт.");
        if (orders.Count > 0)
        {
            Console.WriteLine("\nОжидаются:");
            foreach (var order in orders) Console.WriteLine($"  {order.PartName}: {order.Quantity} шт. (через {order.DeliveryCounter})");
        }
    }

    private string GetRandomPart()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var cmd = new SqlCommand("SELECT Name FROM Parts WHERE IsActive = 1", connection);
        using var reader = cmd.ExecuteReader();
        var parts = new List<string>();
        while (reader.Read()) parts.Add(reader["Name"].ToString());
        return parts.Count > 0 ? parts[random.Next(parts.Count)] : "тормозные колодки";
    }

    private int GetPartPrice(string part)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var cmd = new SqlCommand("SELECT Price FROM Parts WHERE Name = @part", connection);
        cmd.Parameters.AddWithValue("@part", part);
        return (int)(cmd.ExecuteScalar() ?? 500);
    }

    private void AcceptOrder(string brokenPart, int repairCost, int client)
    {
        if (warehouse.GetValueOrDefault(brokenPart) > 0)
        {
            warehouse[brokenPart]--;
            money += repairCost;
            SaveInventory(brokenPart, warehouse[brokenPart]);
            SaveGame();
            LogTransaction(client, brokenPart, repairCost, "success");
            Console.WriteLine($"Успешный ремонт! +{repairCost} руб.");
        }
        else
        {
            Console.WriteLine("Нет детали! Замена случайной...");
            if (warehouse.Count > 0)
            {
                var randomPart = warehouse.Keys.First();
                warehouse[randomPart]--;
                if (warehouse[randomPart] == 0) warehouse.Remove(randomPart);
                int penalty = repairCost + 1000;
                money -= penalty;
                SaveInventory(randomPart, warehouse.GetValueOrDefault(randomPart));
                SaveGame();
                LogTransaction(client, brokenPart, -penalty, "failed");
                Console.WriteLine($"Клиент недоволен! Штраф: {penalty} руб.");
            }
            else
            {
                int penalty = repairCost + 1500;
                money -= penalty;
                SaveGame();
                LogTransaction(client, brokenPart, -penalty, "no_parts");
                Console.WriteLine($"Нет деталей! Штраф: {penalty} руб.");
            }
        }
    }

    private void RefuseOrder(int client)
    {
        money -= 300;
        SaveGame();
        LogTransaction(client, "refusal", -300, "refused");
        Console.WriteLine("Отказ. Штраф: 300 руб.");
    }

    private void ShowPurchaseMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Баланс: {money} руб.\nДоступные запчасти:");
            var parts = GetParts();
            for (int i = 0; i < parts.Count; i++)
                Console.WriteLine($"{i + 1} - {parts[i].Name}: {parts[i].Price} руб.");
            Console.WriteLine($"{parts.Count + 1} - Назад");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice == parts.Count + 1) break;
            if (choice > 0 && choice <= parts.Count)
            {
                var part = parts[choice - 1];
                Console.Write($"Количество {part.Name}: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    int cost = part.Price * quantity;
                    if (cost <= money)
                    {
                        money -= cost;
                        CreateOrder(part.Name, quantity);
                        SaveGame();
                        Console.WriteLine($"Заказ на {quantity} {part.Name} оформлен! -{cost} руб.");
                    }
                    else Console.WriteLine("Недостаточно денег!");
                }
                else Console.WriteLine("Неверное количество!");
            }
            else Console.WriteLine("Неверный выбор!");
            Console.ReadKey();
        }
    }

    private List<Part> GetParts()
    {
        var parts = new List<Part>();
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var cmd = new SqlCommand("SELECT Name, Price FROM Parts WHERE IsActive = 1", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) parts.Add(new Part { Name = reader["Name"].ToString(), Price = (int)reader["Price"] });
        return parts;
    }

    private void CreateOrder(string part, int quantity)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var cmd = new SqlCommand("INSERT INTO PurchaseOrders (GameId, PartName, Quantity, DeliveryCounter) VALUES (@id, @part, @qty, 2)", connection);
        cmd.Parameters.AddWithValue("@id", gameId);
        cmd.Parameters.AddWithValue("@part", part);
        cmd.Parameters.AddWithValue("@qty", quantity);
        cmd.ExecuteNonQuery();
        UpdateOrders();
    }

    private void SaveInventory(string part, int quantity)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var cmd = new SqlCommand(
            "IF EXISTS (SELECT 1 FROM Inventory WHERE GameId = @id AND PartName = @part) " +
            "UPDATE Inventory SET Quantity = @qty WHERE GameId = @id AND PartName = @part " +
            "ELSE INSERT INTO Inventory (GameId, PartName, Quantity) VALUES (@id, @part, @qty)", connection);
        cmd.Parameters.AddWithValue("@id", gameId);
        cmd.Parameters.AddWithValue("@part", part);
        cmd.Parameters.AddWithValue("@qty", quantity);
        cmd.ExecuteNonQuery();
    }

  