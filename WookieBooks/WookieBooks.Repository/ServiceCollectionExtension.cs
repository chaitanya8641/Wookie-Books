using Microsoft.Extensions.DependencyInjection;
using WookieBooks.Repository.Interfaces;
using WookieBooks.Repository.Repositories;

namespace WookieBooks.Repository
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEffCollections(this IServiceCollection services)
        {
            services.AddDbContext<WookieBooksContext>();
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRestrictedUserRepository, RestrictedUserRepository>();
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
