using Microsoft.AspNetCore.Http;

namespace Application.Middleware
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var headers = new Dictionary<string, string>
            {
                {"X-Frame-Options", "DENY"},
                {"X-XSS-Protection", "1; mode=block"},
                {"X-Content-Type-Options", "nosniff"},
                {"X-XSRF-TOKEN", httpContext.Request.Cookies["XSRF-TOKEN"] ?? ""},
                {"Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload"},
                {"X-Download-Options", "noopen"},
                {"Referrer-Policy", "no-referrer"},
                {"X-Permitted-Cross-Domain-Policies", "none"},
                {"Permissions-Policy", "accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'"},
                {"Content-Security-Policy", "form-action 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval' https://www.google.com https://code.jquery.com; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://fonts.gstatic.com https://cdn.jsdelivr.net"}
            };

            foreach (var header in headers.Where(header => !httpContext.Response.Headers.ContainsKey(header.Key)))
                httpContext.Response.Headers.Add(header.Key, header.Value);

            await _next(httpContext);
        }
    }
}
