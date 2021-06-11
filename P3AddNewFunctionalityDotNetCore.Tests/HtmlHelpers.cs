using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public static class HtmlHelpers
    {
        
        public static IHtmlDocument ParseHtml(string htmlContent)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var htmlParser = context.GetService<IHtmlParser>();
            return htmlParser.ParseDocument(htmlContent);
        }
    }
}