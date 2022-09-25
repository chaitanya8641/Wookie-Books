using WookieBooks.Repository.Interfaces;
using WookieBooks.Repository.Repositories;

namespace WookieBooks.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WookieBooksContext _context;
        public UnitOfWork(WookieBooksContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Books = new BookRepository(_context);
            RestrictedUsers = new RestrictedUserRepository(_context);
        }

        public IUserRepository Users { get; private set; }
        public IRestrictedUserRepository RestrictedUsers { get; private set; }
        public IBookRepository Books { get; private set; }

        public async Task<bool> Complete()
        {
            bool returnValue = true;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    returnValue = false;
                    transaction.Rollback();
                }
                finally
                {
                    transaction.Dispose();
                }
            }

            return returnValue;
        }
        public void Dispose()
        {
            Dispose(true);
        }


        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                _context.Dispose();
            }

            _disposedValue = true;
        }
    }
}
