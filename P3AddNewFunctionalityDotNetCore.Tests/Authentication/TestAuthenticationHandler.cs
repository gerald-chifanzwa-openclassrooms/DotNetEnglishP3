using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace P3AddNewFunctionalityDotNetCore.Tests.Authentication
{
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                         ILoggerFactory logger,
                                         UrlEncoder encoder,
                                         ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.NameIdentifier, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TEST");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TEST");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}