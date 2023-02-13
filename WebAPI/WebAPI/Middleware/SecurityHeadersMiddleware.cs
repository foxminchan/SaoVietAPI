namespace WebAPI.Middleware
{
    /// <summary>
    /// Middleware bảo mật HTTP Headers
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Bảo mật HTTP Headers
        /// </summary>
        /// <param name="next">Đối tượng RequestDelegate</param>
        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Thêm HTTP Headers
        /// </summary>
        /// <param name="context">Đối tượng HttpContext</param>
        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self';");
            context.Response.Headers.Add("Referrer-Policy", "no-referrer");
            context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            await _next(context);
        }
    }

    /// <summary>
    /// Middleware bảo mật HTTP Headers
    /// </summary>
    public static class SecurityHeadersMiddlewareExtensions
    {

        /// <summary>
        /// Sử dụng bảo mật HTTP Headers
        /// </summary>
        /// <param name="builder">Đối tượng IApplicationBuilder</param>
        public static void UseSecurityHeadersMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
