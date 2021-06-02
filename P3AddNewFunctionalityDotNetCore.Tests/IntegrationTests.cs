using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class IntegrationTests : IClassFixture<ApplicationFactory>
    {
        private readonly ApplicationFactory _applicationFactory;

        public IntegrationTests(ApplicationFactory applicationFactory)
        {
            _applicationFactory = applicationFactory;
        }

        [Fact]
        public async Task ProductCreation_WhenAdminAddsAProduct_ItShouldAppearInProductsList()
        {
            // Arrange
            var client = _applicationFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var createUri = new Uri("/product/create", UriKind.Relative);
            var listUri = new Uri("/product/index", UriKind.Relative);
            var faker = new Faker();
            var productName = faker.Commerce.ProductName();
            var product = faker.Commerce.Product();
            var description = faker.Commerce.ProductDescription();
            var quantity = faker.Random.Number(1, 100).ToString();
            var price = faker.Commerce.Price(decimals: 0);

            var formContent = new MultipartFormDataContent()
            {
                {new StringContent(price), "Price"},
                {new StringContent(quantity), "Stock"},
                {new StringContent(description), "Description"},
                {new StringContent(product), "Details"},
                {new StringContent(productName), "Name"},
            };

            // Act
            var createResponse = await client.PostAsync(createUri, formContent).ConfigureAwait(false);
            // On success, it redirects to admin page
            createResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);
            
            var listResponse = await client.GetAsync(listUri).ConfigureAwait(false);
            listResponse.EnsureSuccessStatusCode();
            var responseString = await listResponse.Content.ReadAsStringAsync();
            var document = ParseHtml(responseString);
            var productTableRows = document.QuerySelectorAll("table>tbody>tr");
            // Assert
            productTableRows.Should().NotBeEmpty();
            productTableRows.Should().ContainSingle(row => row.TextContent.Contains(productName));
        }

        private static IHtmlDocument ParseHtml(string htmlContent)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var htmlParser = context.GetService<IHtmlParser>();
            return htmlParser.ParseDocument(htmlContent);
        }
    }
}