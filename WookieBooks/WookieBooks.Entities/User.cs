using System.ComponentModel.DataAnnotations;

namespace WookieBooks.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Author_Pseudonym { get; set; } = null!;
    }
}
