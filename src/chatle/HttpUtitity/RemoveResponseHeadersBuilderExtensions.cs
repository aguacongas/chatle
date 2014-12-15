using Microsoft.AspNet.Builder;

namespace ChatLe.HttpUtility
{
    public static class RemoveResponseHeaderBuilderExtensions
    {
        public static IApplicationBuilder UseRemoveResponseHeaders(this IApplicationBuilder app)
        {
            app.UseMiddleware<RemoveResponseHeadersMiddleware>();
            return app;
        }

    }
}