using System.ComponentModel.DataAnnotations;

namespace WookieBooks.Entities
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string CoverImage { get; set; } = null!;
        public string Price { get; set; } = null!;
    }
}

