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

 