using WookieBooks.Entities;
using WookieBooks.Repository.Interfaces;

namespace WookieBooks.Repository.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(WookieBooksContext context) : base(context)
        {

        }      
    }
}
