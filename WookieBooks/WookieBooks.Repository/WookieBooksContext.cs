using Microsoft.EntityFrameworkCore;
using WookieBooks.Entities;

namespace WookieBooks.Repository
{
    public class WookieBooksContext : DbContext
    {
        public WookieBooksContext(DbContextOptions<WookieBooksContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<RestrictedUser> RestrictedUsers { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}
