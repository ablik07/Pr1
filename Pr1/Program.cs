using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement
{
    public enum Genre
    {
        Fiction = 1,
        Science = 2,
        Fantasy = 3,
        Mystery = 4,
        Romance = 5,
        Biography = 6
    }

    public class Book
    {
        private static int _nextId = 1;

        public int Id { get; private set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre Genre { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }

        public Book(string title, string author, Genre genre, int year, decimal price)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new Exception("Название не может быть пустым");
            if (string.IsNullOrWhiteSpace(author)) throw new Exception("Автор не может быть пустым");

            Id = _nextId++;
            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
            Price = price;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Название: {Title}, Автор: {Author}, Жанр: {Genre}, Год: {Year}, Цена: {Price}";
        }
    }

    public class Library
    {
        private List<Book> _books = new List<Book>();

        public void AddBook(Book book) => _books.Add(book);

        public bool RemoveBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _books.Remove(book);
                return true;
            }
            return false;
        }

        public List<Book> FindBooksByTitle(string title) =>
            _books.Where(b => b.Title.ToLower().Contains(title.ToLower())).ToList();

        public List<Book> FindBooksByAuthor(string author) =>
            _books.Where(b => b.Author.ToLower().Contains(author.ToLower())).ToList();

        public List<Book> FindBooksByGenre(Genre genre) =>
            _books.Where(b => b.Genre == genre).ToList();

        public List<Book> SortByTitle() => _books.OrderBy(b => b.Title).ToList();
        public List<Book> SortByYear() => _books.OrderBy(b => b.Year).ToList();
        public List<Book> SortByYearDescending() => _books.OrderByDescending(b => b.Year).ToList();

        public Book GetMostExpensiveBook() => _books.OrderByDescending(b => b.Price).FirstOrDefault();
        public Book GetCheapestBook() => _books.OrderBy(b => b.Price).FirstOrDefault();

        public void ShowBooksByAuthors()
        {
            var groups = _books.GroupBy(b => b.Author);
            foreach (var group in groups)
            {
                Console.WriteLine($"Автор: {group.Key}, Книг: {group.Count()}");
            }
        }

        public void AddTestData()
        {
            AddBook(new Book("Война и мир", "Лев Толстой", Genre.Fiction, 1869, 1200));
            AddBook(new Book("Преступление и наказание", "Федор Достоевский", Genre.Fiction, 1866, 950));
            AddBook(new Book("Мастер и Маргарита", "Михаил Булгаков", Genre.Fiction, 1967, 1100));
        }

        public List<Book> GetAllBooks() => _books;
    }

    class Program
    {
        static void ShowBooks(List<Book> books, string title = "Книги:")
        {
            Console.WriteLine(title);
            if (books.Count == 0)
            {
                Console.WriteLine("Книги не найдены");
                return;
            }

            foreach (var book in books)
            {
                Console.WriteLine(book);
            }
            Console.WriteLine($"Всего: {books.Count} книг");
        }

        static void Main(string[] args)
        {
            Library library = new Library();
            library.AddTestData();

            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1 - Добавить книгу");
                Console.WriteLine("2 - Удалить книгу");
                Console.WriteLine("3 - Найти книги");
                Console.WriteLine("4 - Сортировать книги");
                Console.WriteLine("5 - Цены книг");
                Console.WriteLine("6 - Книги по авторам");
                Console.WriteLine("7 - Все книги");
                Console.WriteLine("0 - Выход");

                Console.Write("Выбор: ");
                string choice = Console.ReadLine();

                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        try
                        {
                            Console.Write("Название: ");
                            string title = Console.ReadLine();
                            Console.Write("Автор: ");
                            string author = Console.ReadLine();
                            Console.Write("Жанр (1-6): ");
                            Genre genre = (Genre)int.Parse(Console.ReadLine());
                            Console.Write("Год: ");
                            int year = int.Parse(Console.ReadLine());
                            Console.Write("Цена: ");
                            decimal price = decimal.Parse(Console.ReadLine());

                            Book book = new Book(title, author, genre, year, price);
                            library.AddBook(book);
                            Console.WriteLine("Книга добавлена");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Ошибка: " + e.Message);
                        }
                        break;

                    case "2":
                        Console.Write("ID книги: ");
                        int id = int.Parse(Console.ReadLine());
                        if (library.RemoveBook(id))
                            Console.WriteLine("Книга удалена");
                        else
                            Console.WriteLine("Книга не найдена");
                        break;

                    case "3":
                        Console.WriteLine("Поиск: 1-по названию, 2-по автору, 3-по жанру");
                        string searchType = Console.ReadLine();

                        if (searchType == "1")
                        {
                            Console.Write("Название: ");
                            ShowBooks(library.FindBooksByTitle(Console.ReadLine()));
                        }
                        else if (searchType == "2")
                        {
                            Console.Write("Автор: ");
                            ShowBooks(library.FindBooksByAuthor(Console.ReadLine()));
                        }
                        else if (searchType == "3")
                        {
                            Console.Write("Жанр (1-6): ");
                            Genre genre = (Genre)int.Parse(Console.ReadLine());
                            ShowBooks(library.FindBooksByGenre(genre));
                        }
                        break;

                    case "4":
                        Console.WriteLine("Сортировка: 1-по названию, 2-по году (возр), 3-по году (убыв)");
                        string sortType = Console.ReadLine();

                        if (sortType == "1") ShowBooks(library.SortByTitle());
                        else if (sortType == "2") ShowBooks(library.SortByYear());
                        else if (sortType == "3") ShowBooks(library.SortByYearDescending());
                        break;

                    case "5":
                        Book expensive = library.GetMostExpensiveBook();
                        Book cheap = library.GetCheapestBook();

                        Console.WriteLine("Самая дорогая книга:");
                        Console.WriteLine(expensive);
                        Console.WriteLine("Самая дешевая книга:");
                        Console.WriteLine(cheap);
                        break;

                    case "6":
                        library.ShowBooksByAuthors();
                        break;

                    case "7":
                        ShowBooks(library.GetAllBooks());
                        break;
                }
            }
        }
    }
}
