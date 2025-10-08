using System;
using System.Collections.Generic;
using System.Linq;

namespace DailyExpenses
{
    public class Expense
    {
        public string Name { get; set; }
        public double Amount { get; set; }

        public Expense(string name, double amount)
        {
            Name = name;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{Name}; {Amount} рублей";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Expense> expenses = new List<Expense>();

            Console.WriteLine("Введите количество операций (от 2 до 40):");
            int n;
            while (!int.TryParse(Console.ReadLine(), out n) || n < 2 || n > 40)
            {
                Console.WriteLine("Неверное значение. Введите число от 2 до 40:");
            }

            Console.WriteLine("Введите траты по шаблону: (Название; Сумма)");
            for (int i = 0; i < n; i++)
            {
                string input;
                while (true)
                {
                    input = Console.ReadLine().Trim();
                    if (input.StartsWith("(") && input.EndsWith(")"))
                    {
                        input = input.Substring(1, input.Length - 2).Trim();
                        string[] parts = input.Split(';');
                        if (parts.Length == 2)
                        {
                            string name = parts[0].Trim();
                            if (double.TryParse(parts[1].Trim(), out double amount) && amount > 0)
                            {
                                expenses.Add(new Expense(name, amount));
                                break;
                            }
                        }
                    }
                    Console.WriteLine("Неверный формат. Введите по шаблону: (Название; Сумма)");
                }
            }

            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Показать все расходы");
                Console.WriteLine("2. Статистика");
                Console.WriteLine("3. Сортировка по сумме");
                Console.WriteLine("4. Конвертация валюты");
                Console.WriteLine("5. Поиск по названию");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("\nВсе расходы:");
                        foreach (var expense in expenses)
                        {
                            Console.WriteLine(expense);
                        }
                        break;
                    case "2":
                        if (expenses.Count > 0)
                        {
                            double total = expenses.Sum(e => e.Amount);
                            double average = total / expenses.Count;
                            double max = expenses.Max(e => e.Amount);
                            double min = expenses.Min(e => e.Amount);

                            Console.WriteLine($"\nСтатистика:");
                            Console.WriteLine($"Общая сумма: {total} рублей");
                            Console.WriteLine($"Средняя сумма: {average:F2} рублей");
                            Console.WriteLine($"Максимальная сумма: {max} рублей");
                            Console.WriteLine($"Минимальная сумма: {min} рублей");
                        }
                        else
                        {
                            Console.WriteLine("Нет данных.");
                        }
                        break;
                    case "3":
                        BubbleSort(expenses);
                        Console.WriteLine("\nРасходы отсортированы по сумме:");
                        foreach (var expense in expenses)
                        {
                            Console.WriteLine(expense);
                        }
                        break;
                    case "4":
                        Console.WriteLine("\nВыберите валюту для конвертации:");
                        Console.WriteLine("1. Доллар (курс: 90 рублей за 1 доллар)");
                        Console.WriteLine("2. Евро (курс: 100 рублей за 1 евро)");
                        Console.WriteLine("3. Ввести свой курс");
                        Console.Write("Выбор: ");
                        string currencyChoice = Console.ReadLine();
                        double rate = 1.0;
                        string currency = "";

                        switch (currencyChoice)
                        {
                            case "1":
                                rate = 90.0;
                                currency = "долларов";
                                break;
                            case "2":
                                rate = 100.0;
                                currency = "евро";
                                break;
                            case "3":
                                Console.Write("Введите курс: ");
                                if (double.TryParse(Console.ReadLine(), out rate) && rate > 0)
                                {
                                    Console.Write("Введите название валюты: ");
                                    currency = Console.ReadLine().Trim();
                                }
                                else
                                {
                                    Console.WriteLine("Неверный курс.");
                                    continue;
                                }
                                break;
                            default:
                                Console.WriteLine("Неверный выбор.");
                                continue;
                        }

                        Console.WriteLine($"\nРасходы в {currency}:");
                        foreach (var expense in expenses)
                        {
                            double converted = expense.Amount / rate;
                            Console.WriteLine($"{expense.Name}; {converted:F2} {currency}");
                        }
                        break;
                    case "5":
                        Console.Write("Введите название для поиска: ");
                        string search = Console.ReadLine().Trim();
                        var found = expenses.Where(e => e.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                        if (found.Count > 0)
                        {
                            Console.WriteLine("\nНайденные расходы:");
                            foreach (var expense in found)
                            {
                                Console.WriteLine(expense);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ничего не найдено.");
                        }
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
        }

        static void BubbleSort(List<Expense> list)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (list[j].Amount > list[j + 1].Amount)
                    {
                        Expense temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
        }
    }
}