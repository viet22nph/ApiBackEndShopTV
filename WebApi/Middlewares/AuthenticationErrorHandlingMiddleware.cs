using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace WebApi.Middlewares
{
    public class AuthenticationErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (ex is SecurityTokenExpiredException || ex is SecurityTokenInvalidSignatureException || ex is SecurityTokenException)
            {
                await HandleAuthenticationExceptionAsync(context, ex);
            }
        }

        private Task HandleAuthenticationExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var result = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Authentication error",
                Details = exception.Message
            };

            return context.Response.WriteAsJsonAsync(result);
        }
    }
}
