

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WookieBooks.Repository;

namespace WookieBooks.Tests
{
    public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<WookieBooksContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<WookieBooksContext>(options =>
                {
                    options.UseInMemoryDatabase("WookieBooks");
                });

            });
        }
    }
}
