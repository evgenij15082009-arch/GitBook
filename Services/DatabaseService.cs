using Dapper;
using GitBook.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;

namespace GitBook.Services
{
    public static class DatabaseService
    {
        private const string DbName = "pink_library.db";
        private static string ConnectionString => $"Data Source={DbName}";

        public static void Initialize()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE NOT NULL,
                    PasswordHash TEXT NOT NULL
                );");

            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    Genre TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    IsFavorite INTEGER DEFAULT 0,
                    UserId INTEGER DEFAULT 1
                );");

            var count = connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Books");
            if (count == 0)
            {
                connection.Execute(@"
                    INSERT INTO Books (Title, Author, Genre, Content) VALUES 
                    ('Розовая Пантера', 'Геля Системник', 'Детектив', 'Это была самая розовая библиотека в мире...');");
            }
        }

        public static User? Register(string username, string password)
        {
            using var connection = new SqliteConnection(ConnectionString);
            try
            {
                connection.Execute("INSERT INTO Users (Username, PasswordHash) VALUES (@username, @password)", new { username, password });
                return Login(username, password);
            }
            catch { return null; }
        }

        public static User? Login(string username, string password)
        {
            using var connection = new SqliteConnection(ConnectionString);
            return connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Username = @username AND PasswordHash = @password", new { username, password });
        }

        public static IEnumerable<Book> GetAllBooks()
        {
            using var connection = new SqliteConnection(ConnectionString);
            return connection.Query<Book>("SELECT * FROM Books");
        }

        public static void AddBook(Book book)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Execute("INSERT INTO Books (Title, Author, Genre, Content, UserId) VALUES (@Title, @Author, @Genre, @Content, @UserId)", book);
        }

        public static void ToggleFavorite(int bookId, bool isFavorite)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Execute("UPDATE Books SET IsFavorite = @isFavorite WHERE Id = @bookId", new { isFavorite = isFavorite ? 1 : 0, bookId });
        }
    }
}