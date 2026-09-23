using GitBook.Models;
using GitBook.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;

namespace GitBook.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Book> _books = new();
        private string _searchQuery = string.Empty;
        private Book? _selectedBook;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _isLoggedIn = false;
        private string _authMessage = string.Empty;

        private string _newTitle = string.Empty;
        private string _newAuthor = string.Empty;
        private string _newGenre = string.Empty;
        private string _newContent = string.Empty;

        public ObservableCollection<Book> Books
        {
            get => _books;
            set { _books = value; OnPropertyChanged(); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); ApplySearch(); }
        }

        public Book? SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public bool IsLoggedIn { get => _isLoggedIn; set { _isLoggedIn = value; OnPropertyChanged(); } }
        public string AuthMessage { get => _authMessage; set { _authMessage = value; OnPropertyChanged(); } }

        public string NewTitle { get => _newTitle; set { _newTitle = value; OnPropertyChanged(); } }
        public string NewAuthor { get => _newAuthor; set { _newAuthor = value; OnPropertyChanged(); } }
        public string NewGenre { get => _newGenre; set { _newGenre = value; OnPropertyChanged(); } }
        public string NewContent { get => _newContent; set { _newContent = value; OnPropertyChanged(); } }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }

        public MainViewModel()
        {
            DatabaseService.Initialize();

            LoginCommand = new RelayCommand(_ =>
            {
                var user = DatabaseService.Login(Username, Password);
                if (user != null)
                {
                    IsLoggedIn = true;
                    LoadBooks();
                }
                else { AuthMessage = "❌ Неверный логин или пароль!"; }
            });

            RegisterCommand = new RelayCommand(_ =>
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    AuthMessage = "Заполните все поля!";
                    return;
                }
                var user = DatabaseService.Register(Username, Password);
                if (user != null)
                {
                    IsLoggedIn = true;
                    LoadBooks();
                }
                else { AuthMessage = "Пользователь уже существует!"; }
            });

            AddBookCommand = new RelayCommand(_ =>
            {
                if (!string.IsNullOrWhiteSpace(NewTitle) && !string.IsNullOrWhiteSpace(NewContent))
                {
                    DatabaseService.AddBook(new Book { Title = NewTitle, Author = NewAuthor, Genre = NewGenre, Content = NewContent });
                    NewTitle = NewAuthor = NewGenre =

NewContent = string.Empty;
                    LoadBooks();
                }
            });

            ToggleFavoriteCommand = new RelayCommand(obj =>
            {
                if (obj is Book book)
                {
                    book.IsFavorite = !book.IsFavorite;
                    DatabaseService.ToggleFavorite(book.Id, book.IsFavorite);
                    LoadBooks();
                }
            });
        }

        private void LoadBooks()
        {
            Books = new ObservableCollection<Book>(DatabaseService.GetAllBooks());
        }

        private void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery)) LoadBooks();
            else
            {
                var filtered = DatabaseService.GetAllBooks().Where(b =>
                    b.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
                Books = new ObservableCollection<Book>(filtered);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        public RelayCommand(Action<object?> execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}