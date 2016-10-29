using System;
using Chatle.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods to add Microsoft Account authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class ChatleMicrosoftAccountAppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="ChatleMicrosoftAccountMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>, which enables Microsoft Account authentication capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseChatleMicrosoftAccountAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ChatleMicrosoftAccountMiddleware>();
        }

        /// <summary>
        /// Adds the <see cref="ChatleMicrosoftAccountMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>, which enables Microsoft Account authentication capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="options">A <see cref="MicrosoftAccountOptions"/> that specifies options for the middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseChatleMicrosoftAccountAuthentication(this IApplicationBuilder app, MicrosoftAccountOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<ChatleMicrosoftAccountMiddleware>(Options.Create(options));
        }
    }
}
