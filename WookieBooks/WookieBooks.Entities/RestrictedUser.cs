using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WookieBooks.Entities
{
    public class RestrictedUser
    {
        [Key]
        public int RestrictedUserId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public ICollection<User>? Users { get; set; }

    }
}
