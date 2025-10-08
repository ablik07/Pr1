using System;
using System.Collections.Generic;

namespace TextAnalyzer
{
    class Program
    {
        static List<TextData> allTexts = new List<TextData>();

        class TextData
        {
            public string Text;
            public int Words;
            public string ShortWord;
            public string LongWord;
            public int Sentences;
            public int Vowels;
            public int Consonants;
            public DateTime Date;
        }

        static void Main()
        {
            Console.WriteLine("Анализатор текста");

            while (true)
            {
                Console.WriteLine("\n1 - Анализ");
                Console.WriteLine("2 - История");
                Console.WriteLine("3 - Выход");
                Console.Write("Ваш выбор: ");

                string choice = Console.ReadLine();

                if (choice == "1") Analyze();
                else if (choice == "2") ShowHistory();
                else if (choice == "3") break;
                else Console.WriteLine("Неверный выбор");
            }
        }

        static void Analyze()
        {
            Console.WriteLine("\nВведите текст (от 100 символов):");
            string text = Console.ReadLine();

            if (text == null || text.Length < 100)
            {
                Console.WriteLine("Мало символов");
                return;
            }

            TextData data = new TextData();
            data.Text = text;
            data.Date = DateTime.Now;

            // Подсчет слов
            string[] words = text.Split(' ', ',', '.', '!', '?', ';', ':', '-', '\n', '\r', '\t');
            data.Words = 0;
            data.ShortWord = "";
            data.LongWord = "";

            foreach (string word in words)
            {
                if (word.Length > 0)
                {
                    data.Words++;

                    if (data.ShortWord == "" || word.Length < data.ShortWord.Length)
                        data.ShortWord = word;

                    if (word.Length > data.LongWord.Length)
                        data.LongWord = word;
                }
            }

            // Подсчет предложений
            data.Sentences = 0;
            foreach (char c in text)
            {
                if (c == '.' || c == '!' || c == '?')
                    data.Sentences++;
            }

            // Подсчет букв
            data.Vowels = 0;
            data.Consonants = 0;
            string vowelLetters = "аеёиоуыэюя";

            foreach (char c in text.ToLower())
            {
                if (char.IsLetter(c))
                {
                    if (vowelLetters.Contains(c.ToString()))
                        data.Vowels++;
                    else
                        data.Consonants++;
                }
            }

            allTexts.Add(data);
            ShowResults(data);
        }

        static void ShowResults(TextData data)
        {
            Console.WriteLine("\nРезультаты:");
            Console.WriteLine("Слов: " + data.Words);
            Console.WriteLine("Предложений: " + data.Sentences);
            Console.WriteLine("Короткое слово: " + data.ShortWord);
            Console.WriteLine("Длинное слово: " + data.LongWord);
            Console.WriteLine("Гласные: " + data.Vowels);
            Console.WriteLine("Согласные: " + data.Consonants);
        }

        static void ShowHistory()
        {
            if (allTexts.Count == 0)
            {
                Console.WriteLine("Нет данных");
                return;
            }

            Console.WriteLine("\nИстория анализов:");

            for (int i = 0; i < allTexts.Count; i++)
            {
                TextData data = allTexts[i];
                string preview = data.Text.Length > 50 ? data.Text.Substring(0, 50) + "..." : data.Text;

                Console.WriteLine($"{i + 1}. {preview}");
                Console.WriteLine($"   Слов: {data.Words}, Предложений: {data.Sentences}");
            }
        }
    }
}