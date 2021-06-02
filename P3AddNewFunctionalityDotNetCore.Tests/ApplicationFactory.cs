using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Tests.Authentication;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                // Mock Authentication
                services.AddAuthentication(options => { options.DefaultAuthenticateScheme = "TEST"; })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("TEST", o => { });

                // Remove Registered DbContextOptionsBuilder
                var dbContextBuilderService =
                    services.FirstOrDefault(service => service.ServiceType ==
                                                       typeof(DbContextOptions<AppIdentityDbContext>));
                services.Remove(dbContextBuilderService);

                services.AddDbContext<AppIdentityDbContext>(options => options.UseInMemoryDatabase("TESTS"));
            });
        }
    }
}