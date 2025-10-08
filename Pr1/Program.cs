using System;
using System.Collections.Generic;

namespace TextAnalyzer
{
    class Program
    {
        class TextInfo
        {
            public string Text;
            public int Words;
            public string ShortWord;
            public string LongWord;
            public int Sentences;
            public int Vowels;
            public int Consonants;
            public Dictionary<char, int> Letters = new Dictionary<char, int>();
            public DateTime Date;
        }

        static List<TextInfo> allTexts = new List<TextInfo>();
        static char[] vowels = { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };

        static void Main(string[] args)
        {
            Console.WriteLine("Анализатор текста");

            while (true)
            {
                Console.WriteLine("\n1. Новый текст");
                Console.WriteLine("2. История");
                Console.WriteLine("3. Выход");
                Console.Write("Выбор: ");

                string choice = Console.ReadLine();

                if (choice == "1") NewText();
                else if (choice == "2") ShowAll();
                else if (choice == "3") break;
                else Console.WriteLine("Ошибка");
            }
        }

        static void NewText()
        {
            Console.WriteLine("\nВведите текст (от 100 символов):");
            string text = Console.ReadLine();

            if (text == null || text.Length < 100)
            {
                Console.WriteLine("Мало символов");
                return;
            }

            TextInfo info = new TextInfo();
            info.Text = text;
            info.Date = DateTime.Now;

            Analyze(text, info);
            allTexts.Add(info);
            ShowText(info);
        }

        static void Analyze(string text, TextInfo info)
        {
            // Слова
            string[] words = text.Split(' ', ',', '.', '!', '?', ';', ':', '-', '\n', '\r', '\t');
            info.Words = 0;
            info.ShortWord = "";
            info.LongWord = "";

            foreach (string word in words)
            {
                if (word.Length > 0)
                {
                    info.Words++;
                    if (info.ShortWord == "" || word.Length < info.ShortWord.Length) info.ShortWord = word;
                    if (word.Length > info.LongWord.Length) info.LongWord = word;
                }
            }

            // Предложения
            info.Sentences = 0;
            foreach (char c in text)
            {
                if (c == '.' || c == '!' || c == '?') info.Sentences++;
            }

            // Буквы
            info.Vowels = 0;
            info.Consonants = 0;
            info.Letters.Clear();

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    char lower = char.ToLower(c);
                    bool isVowel = false;

                    foreach (char v in vowels)
                    {
                        if (lower == v)
                        {
                            isVowel = true;
                            break;
                        }
                    }

                    if (isVowel) info.Vowels++;
                    else info.Consonants++;

                    if (info.Letters.ContainsKey(lower)) info.Letters[lower]++;
                    else info.Letters[lower] = 1;
                }
            }
        }

        static void ShowText(TextInfo info)
        {
            Console.WriteLine($"\nДата: {info.Date}");
            Console.WriteLine($"Символов: {info.Text.Length}");
            Console.WriteLine($"Слов: {info.Words}");
            Console.WriteLine($"Предложений: {info.Sentences}");
            Console.WriteLine($"Гласные: {info.Vowels}");
            Console.WriteLine($"Согласные: {info.Consonants}");
            Console.WriteLine($"Короткое: {info.ShortWord}");
            Console.WriteLine($"Длинное: {info.LongWord}");

            Console.WriteLine("Буквы:");
            foreach (var letter in info.Letters)
            {
                double percent = (double)letter.Value / (info.Vowels + info.Consonants) * 100;
                Console.WriteLine($"  {letter.Key}: {letter.Value} ({percent:F1}%)");
            }
        }

        static void ShowAll()
        {
            if (allTexts.Count == 0)
            {
                Console.WriteLine("\nНет данных");
                return;
            }

            Console.WriteLine($"\nВсего текстов: {allTexts.Count}");

            for (int i = 0; i < allTexts.Count; i++)
            {
                TextInfo info = allTexts[i];
                string preview = info.Text.Length > 50 ? info.Text.Substring(0, 50) + "..." : info.Text;
                Console.WriteLine($"\nТекст {i + 1}: {preview}");
                Console.WriteLine($"Слов: {info.Words}, Предложений: {info.Sentences}");
            }

            int totalWords = 0;
            foreach (TextInfo info in allTexts)
            {
                totalWords += info.Words;
            }
            Console.WriteLine($"\nВсего слов: {totalWords}");
        }
    }
}