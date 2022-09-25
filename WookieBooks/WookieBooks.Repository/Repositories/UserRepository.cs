using WookieBooks.Entities;
using WookieBooks.Repository.Interfaces;

namespace WookieBooks.Repository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(WookieBooksContext context) : base(context)
        {

        }
    }
}
