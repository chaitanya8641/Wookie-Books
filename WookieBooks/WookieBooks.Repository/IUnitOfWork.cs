using WookieBooks.Repository.Interfaces;

namespace WookieBooks.Repository
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRestrictedUserRepository RestrictedUsers { get; }
        IBookRepository Books { get; }
        Task<bool> Complete();
    }
}
