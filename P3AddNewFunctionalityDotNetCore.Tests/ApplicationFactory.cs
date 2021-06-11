using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Data;
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
                ConfigureTestDbContext<AppIdentityDbContext>(services);
                ConfigureTestDbContext<P3Referential>(services);
            });
        }

        public WebApplicationFactory<Startup> WithAuthenticatedUser()
        {
            return WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Register a mock Authentication Scheme which always returns an authenticated user
                    services.AddAuthentication(options => options.DefaultAuthenticateScheme = "TEST")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("TEST", null);
                });
            });
        }

        public static void ConfigureTestDbContext<T>(IServiceCollection services) where T : DbContext
        {
            // Remove Registered DbContextOptionsBuilder
            var dbContextBuilderService =
                services.FirstOrDefault(service => service.ServiceType == typeof(DbContextOptions<T>));
            services.Remove(dbContextBuilderService);

            services.AddDbContext<T>(options => options.UseInMemoryDatabase($"{DateTime.Now:u}"));
        }
    }
}