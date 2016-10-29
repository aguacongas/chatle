using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Newtonsoft.Json.Linq;

namespace Chatle.Authentication
{
    internal class ChatleMicrosoftAccountHandler : OAuthHandler<MicrosoftAccountOptions>
    {
        private readonly IHostingEnvironment _env;

        public ChatleMicrosoftAccountHandler(HttpClient httpClient, IHostingEnvironment env)
            : base(httpClient)
        {
            _env = env;
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrived Microsoft user information ({response.StatusCode}) Please check if the authentication information is correct and the corresponding Microsoft Account API is enabled.");
            }

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), properties, Options.AuthenticationScheme);
            var context = new OAuthCreatingTicketContext(ticket, Context, Options, Backchannel, tokens, payload);
            var identifier = MicrosoftAccountHelper.GetId(payload);
            if (!string.IsNullOrEmpty(identifier))
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier, ClaimValueTypes.String, Options.ClaimsIssuer));
                identity.AddClaim(new Claim("urn:microsoftaccount:id", identifier, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            var name = MicrosoftAccountHelper.GetDisplayName(payload);
            if (!string.IsNullOrEmpty(name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, Options.ClaimsIssuer));
                identity.AddClaim(new Claim("urn:microsoftaccount:name", name, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            var givenName = MicrosoftAccountHelper.GetGivenName(payload);
            if (!string.IsNullOrEmpty(givenName))
            {
                identity.AddClaim(new Claim(ClaimTypes.GivenName, givenName, ClaimValueTypes.String, Options.ClaimsIssuer));
                identity.AddClaim(new Claim("urn:microsoftaccount:givenname", givenName, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            var surname = MicrosoftAccountHelper.GetSurname(payload);
            if (!string.IsNullOrEmpty(surname))
            {
                identity.AddClaim(new Claim(ClaimTypes.Surname, surname, ClaimValueTypes.String, Options.ClaimsIssuer));
                identity.AddClaim(new Claim("urn:microsoftaccount:surname", surname, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            var email = MicrosoftAccountHelper.GetEmail(payload);
            if (!string.IsNullOrEmpty(email))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            await Options.Events.CreatingTicket(context);
            return context.Ticket;
        }

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            return await base.ExchangeCodeAsync(code, FixRedirectUri(redirectUri));
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            return base.BuildChallengeUrl(properties, FixRedirectUri(redirectUri));
        }

        private string FixRedirectUri(string redirectUri)
        {
            if(_env.IsProduction())
            {
                return redirectUri.Replace("http://", "https://");
            }
            return redirectUri;
        }
    }
}