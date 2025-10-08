using System;
using System.Collections.Generic;
using System.Linq;

public enum Category
{
    Электроника,
    Одежда,
    Продукты,
    Книги,
    Спорт
}

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Count { get; set; }
    public bool Available { get; set; }
    public Category Type { get; set; }

    public Product(string id, string name, decimal price, int count, Category type)
    {
        Id = id;
        Name = name;
        Price = price;
        Count = count;
        Available = count > 0;
        Type = type;
    }

    public void ShowInfo()
    {
        Console.WriteLine($"\nКод: {Id}");
        Console.WriteLine($"Название: {Name}");
        Console.WriteLine($"Цена: {Price} руб.");
        Console.WriteLine($"Количество: {Count}");
        Console.WriteLine($"Наличие: {(Available ? "Есть" : "Нет")}");
        Console.WriteLine($"Категория: {Type}");
        Console.WriteLine($"Общая стоимость: {Price * Count} руб.");
    }

    public void UpdateAvailability()
    {
        Available = Count > 0;
    }
}

class Program
{
    static List<Product> products = new List<Product>();
    static int counter = 1;

    static void Main(string[] args)
    {
        bool work = true;

        while (work)
        {
            Console.WriteLine("\nУправление складом");
            Console.WriteLine("1. Добавить товар");
            Console.WriteLine("2. Удалить товар");
            Console.WriteLine("3. Пополнить склад");
            Console.WriteLine("4. Продать товар");
            Console.WriteLine("5. Поиск");
            Console.WriteLine("6. Все товары");
            Console.WriteLine("7. Выход");
            Console.Write("Выберите: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AddProduct(); break;
                case "2": DeleteProduct(); break;
                case "3": Restock(); break;
                case "4": Sell(); break;
                case "5": Find(); break;
                case "6": ShowAll(); break;
                case "7": work = false; break;
                default: Console.WriteLine("Ошибка"); break;
            }
        }
    }

    static string CreateId()
    {
        return "1" + counter++.ToString("D3");
    }

    static void AddProduct()
    {
        try
        {
            Console.WriteLine("\nНовый товар");

            string id = CreateId();
            Console.WriteLine($"Код: {id}");

            Console.Write("Название: ");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Введите название");
                return;
            }

            Console.Write("Цена: ");
            decimal price = decimal.Parse(Console.ReadLine());

            if (price <= 0)
            {
                Console.WriteLine("Цена должна быть больше 0");
                return;
            }

            Console.Write("Количество: ");
            int count = int.Parse(Console.ReadLine());

            if (count < 0)
            {
                Console.WriteLine("Количество не может быть отрицательным");
                return;
            }

            Console.WriteLine("\nКатегории:");
            foreach (var category in Enum.GetValues(typeof(Category)))
            {
                Console.WriteLine($"{(int)category}. {category}");
            }

            Console.Write("Выберите категорию: ");
            int catIndex = int.Parse(Console.ReadLine());

            if (!Enum.IsDefined(typeof(Category), catIndex))
            {
                Console.WriteLine("Неверная категория");
                return;
            }

            Category cat = (Category)catIndex;
            Product newProduct = new Product(id, name, price, count, cat);
            products.Add(newProduct);

            Console.WriteLine("Товар добавлен!");
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка ввода");
        }
    }

    static void DeleteProduct()
    {
        if (!products.Any())
        {
            Console.WriteLine("Список пуст");
            return;
        }

        Console.WriteLine("\nУдаление");
        Console.Write("Введите код: ");
        string id = Console.ReadLine();

        Product product = products.FirstOrDefault(p => p.Id == id);

        if (product != null)
        {
            products.Remove(product);
            Console.WriteLine("Товар удален");
        }
        else
        {
            Console.WriteLine("Товар не найден");
        }
    }

    static void Restock()
    {
        if (!products.Any())
        {
            Console.WriteLine("Список пуст");
            return;
        }

        try
        {
            Console.WriteLine("\nПополнение");
            Console.Write("Введите код: ");
            string id = Console.ReadLine();

            Product product = products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                Console.Write($"Текущее количество: {product.Count}");
                Console.Write("\nДобавить: ");
                int add = int.Parse(Console.ReadLine());

                if (add <= 0)
                {
                    Console.WriteLine("Введите положительное число");
                    return;
                }

                product.Count += add;
                product.UpdateAvailability();
                Console.WriteLine("Склад пополнен");
            }
            else
            {
                Console.WriteLine("Товар не найден");
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка ввода");
        }
    }

    static void Sell()
    {
        if (!products.Any())
        {
            Console.WriteLine("Список пуст");
            return;
        }

        try
        {
            Console.WriteLine("\nПродажа");
            Console.Write("Введите код: ");
            string id = Console.ReadLine();

            Product product = products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                if (!product.Available)
                {
                    Console.WriteLine("Товара нет в наличии");
                    return;
                }

                Console.Write($"В наличии: {product.Count}");
                Console.Write("\nПродать: ");
                int sellCount = int.Parse(Console.ReadLine());

                if (sellCount <= 0)
                {
                    Console.WriteLine("Введите положительное число");
                    return;
                }

                if (sellCount > product.Count)
                {
                    Console.WriteLine("Недостаточно товара");
                    return;
                }

                product.Count -= sellCount;
                product.UpdateAvailability();

                decimal total = sellCount * product.Price;
                Console.WriteLine($"Сумма продажи: {total} руб.");
            }
            else
            {
                Console.WriteLine("Товар не найден");
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка ввода");
        }
    }

    static void Find()
    {
        if (!products.Any())
        {
            Console.WriteLine("Список пуст");
            return;
        }

        Console.WriteLine("\nПоиск");
        Console.WriteLine("1. По коду");
        Console.WriteLine("2. По названию");
        Console.WriteLine("3. По категории");
        Console.Write("Выберите: ");

        string type = Console.ReadLine();
        List<Product> found = new List<Product>();

        switch (type)
        {
            case "1":
                Console.Write("Код: ");
                string id = Console.ReadLine();
                found = products.Where(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase)).ToList();
                break;

            case "2":
                Console.Write("Название: ");
                string name = Console.ReadLine();
                found = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
                break;

            case "3":
                Console.WriteLine("\nКатегории:");
                foreach (var category in Enum.GetValues(typeof(Category)))
                {
                    Console.WriteLine($"{(int)category}. {category}");
                }
                Console.Write("Номер категории: ");
                if (int.TryParse(Console.ReadLine(), out int catIndex) && Enum.IsDefined(typeof(Category), catIndex))
                {
                    Category cat = (Category)catIndex;
                    found = products.Where(p => p.Type == cat).ToList();
                }
                else
                {
                    Console.WriteLine("Ошибка");
                    return;
                }
                break;

            default:
                Console.WriteLine("Ошибка");
                return;
        }

        if (found.Any())
        {
            Console.WriteLine($"\nНайдено: {found.Count}");
            foreach (var product in found)
            {
                product.ShowInfo();
            }
        }
        else
        {
            Console.WriteLine("Ничего не найдено");
        }
    }

    static void ShowAll()
    {
        if (!products.Any())
        {
            Console.WriteLine("Список пуст");
            return;
        }

        Console.WriteLine("\nВсе товары");

        var byCategory = products.GroupBy(p => p.Type).OrderBy(g => g.Key);

        foreach (var group in byCategory)
        {
            Console.WriteLine($"\n--- {group.Key} ---");
            foreach (var product in group.OrderBy(p => p.Name))
            {
                Console.WriteLine($"Код: {product.Id}, Название: {product.Name}, Цена: {product.Price}, Количество: {product.Count}");
            }
        }

        Console.WriteLine($"\nИтого:");
        Console.WriteLine($"Всего товаров: {products.Count}");
        Console.WriteLine($"Доступно: {products.Count(p => p.Available)}");
        Console.WriteLine($"Общая стоимость: {products.Sum(p => p.Price * p.Count)} руб.");
    }
}