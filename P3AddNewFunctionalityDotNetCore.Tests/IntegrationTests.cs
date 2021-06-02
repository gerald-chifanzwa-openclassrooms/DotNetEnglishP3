using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Tests.Authentication;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _applicationFactory;

        public IntegrationTests(WebApplicationFactory<Startup> applicationFactory)
        {
            _applicationFactory = applicationFactory;
        }


        [Fact]
        public async Task ProductCreation_WhenAdminAddsAProduct_ItShouldAppearInProductsList()
        {
            // Arrange
            var factory = _applicationFactory.WithWebHostBuilder(builder =>
            {
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
            });
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var createUri = new Uri("/product/create", UriKind.Relative);
            var listUri = new Uri("/product/index", UriKind.Relative);

            var formContent = new MultipartFormDataContent()
            {
                {new StringContent("10"), "Price"},
                {new StringContent("20"), "Stock"},
                {new StringContent("Some product"), "Description"},
                {new StringContent("Product"), "Details"},
                {new StringContent("Random Product 1"), "Name"},
            };

            // Act
            var createResponse = await client.PostAsync(createUri, formContent).ConfigureAwait(false);
            // On success, it redirects to admin page
            createResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);
            
            var listResponse = await client.GetAsync(listUri).ConfigureAwait(false);
            listResponse.EnsureSuccessStatusCode();

            var responseString = await listResponse.Content.ReadAsStringAsync();

            // Assert
            responseString.Should().Contain("Random Product 1");
        }
    }
}