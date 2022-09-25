using WookieBooks.Entities;
using WookieBooks.Repository.Interfaces;

namespace WookieBooks.Repository.Repositories
{
    public class RestrictedUserRepository : GenericRepository<RestrictedUser>, IRestrictedUserRepository
    {
        public RestrictedUserRepository(WookieBooksContext context) : base(context)
        {

        }
    }
}
