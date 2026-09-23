namespace GitBook.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
        public int UserId { get; set; }
    }
}