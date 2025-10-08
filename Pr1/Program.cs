using System;
using System.Collections.Generic;

namespace Library
{
    public class Book
    {
        public int Id;
        public string Title;
        public string Author;
        public string Genre;
        public int Year;
        public decimal Price;

        public override string ToString()
        {
            return $"{Id}. {Title} - {Author} ({Year}) {Price} руб.";
        }
    }

    class Program
    {
        static List<Book> books = new List<Book>();
        static int nextId = 1;

        static void Main()
        {
            AddTestBooks();

            while (true)
            {
                Console.WriteLine("\nБиблиотека");
                Console.WriteLine("1. Все книги");
                Console.WriteLine("2. Добавить книгу");
                Console.WriteLine("3. Удалить книгу");
                Console.WriteLine("4. Найти книгу");
                Console.WriteLine("0. Выход");

                Console.Write("Выбор: ");
                string choice = Console.ReadLine();

                if (choice == "0") break;

                if (choice == "1") ShowAllBooks();
                else if (choice == "2") AddBook();
                else if (choice == "3") RemoveBook();
                else if (choice == "4") FindBook();
            }
        }

        static void ShowAllBooks()
        {
            Console.WriteLine("\nВсе книги:");
            if (books.Count == 0)
            {
                Console.WriteLine("Нет книг");
                return;
            }

            foreach (var book in books)
            {
                Console.WriteLine(book);
            }
        }

        static void AddBook()
        {
            Book book = new Book();
            book.Id = nextId++;

            Console.Write("Название: ");
            book.Title = Console.ReadLine();

            Console.Write("Автор: ");
            book.Author = Console.ReadLine();

            Console.Write("Жанр: ");
            book.Genre = Console.ReadLine();

            Console.Write("Год: ");
            book.Year = int.Parse(Console.ReadLine());

            Console.Write("Цена: ");
            book.Price = decimal.Parse(Console.ReadLine());

            books.Add(book);
            Console.WriteLine("Книга добавлена");
        }

        static void RemoveBook()
        {
            Console.Write("ID книги для удаления: ");
            int id = int.Parse(Console.ReadLine());

            for (int i = 0; i < books.Count; i++)
            {
                if (books[i].Id == id)
                {
                    books.RemoveAt(i);
                    Console.WriteLine("Книга удалена");
                    return;
                }
            }
            Console.WriteLine("Книга не найдена");
        }

        static void FindBook()
        {
            Console.Write("Поиск (название или автор): ");
            string search = Console.ReadLine().ToLower();

            bool found = false;
            foreach (var book in books)
            {
                if (book.Title.ToLower().Contains(search) || book.Author.ToLower().Contains(search))
                {
                    Console.WriteLine(book);
                    found = true;
                }
            }

            if (!found) Console.WriteLine("Ничего не найдено");
        }

        static void AddTestBooks()
        {
            books.Add(new Book { Id = nextId++, Title = "Война и мир", Author = "Лев Толстой", Genre = "Роман", Year = 1869, Price = 1200 });
            books.Add(new Book { Id = nextId++, Title = "Преступление и наказание", Author = "Федор Достоевский", Genre = "Роман", Year = 1866, Price = 950 });
            books.Add(new Book { Id = nextId++, Title = "Мастер и Маргарита", Author = "Михаил Булгаков", Genre = "Роман", Year = 1967, Price = 1100 });
        }
    }
}