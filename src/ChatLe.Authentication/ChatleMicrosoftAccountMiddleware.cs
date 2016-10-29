using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chatle.Authentication
{
    public class ChatleMicrosoftAccountMiddleware: MicrosoftAccountMiddleware 
    {
        private readonly IHostingEnvironment _env;

        public ChatleMicrosoftAccountMiddleware(
            RequestDelegate next,
            IDataProtectionProvider dataProtectionProvider,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            IOptions<SharedAuthenticationOptions> sharedOptions,
            IOptions<MicrosoftAccountOptions> options,
            IHostingEnvironment env)
            : base(next, dataProtectionProvider, loggerFactory, encoder, sharedOptions, options)
        { 
            _env = env;
        }

        
    protected override AuthenticationHandler<MicrosoftAccountOptions> CreateHandler()
        {
            return new ChatleMicrosoftAccountHandler(Backchannel, _env);
        }
    }
}