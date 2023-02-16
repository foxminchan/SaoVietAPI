using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace WebAPI.Middlewares
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    /// <summary>
    /// Middleware thêm các link vào các response trả về
    /// </summary>
    public class HateoasMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LinkGenerator _linkGenerator;

        /// <summary>
        /// Khởi tạo đối tượng HateoasMiddleware
        /// </summary>
        /// <param name="next">Đối tượng RequestDelegate</param>
        /// <param name="linkGenerator">Đối tượng linkGenerator</param>
        public HateoasMiddleware(RequestDelegate next, LinkGenerator linkGenerator)
        {
            _next = next;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Thiết lập các link cho các response trả về
        /// </summary>
        /// <param name="context">Đối tượng HttpContext</param>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
            if (context.Response.StatusCode == 200 && context.Response.ContentType == "application/json")
            {
                var response = context.Response;
                var originalBodyStream = response.Body;
                try
                {
                    using var responseBody = new MemoryStream();
                    response.Body = responseBody;
                    await _next(context);
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
                    var jObject = JObject.Parse(responseBodyText);
                    var links = new JObject();
                    if (jObject["id"] != null)
                    {
                        var id = (jObject["id"] ?? Guid.NewGuid()).Value<string>();
                        links.Add("self", _linkGenerator.GetUriByAction(context, context.GetRouteData().Values["action"]?.ToString(), context.GetRouteData().Values["controller"]?.ToString(), new { id }));
                    }
                    else
                        links.Add("self", _linkGenerator.GetUriByAction(context, context.GetRouteData().Values["action"]?.ToString(), context.GetRouteData().Values["controller"]?.ToString()));

                    jObject.Add("links", links);
                    var output = JsonConvert.SerializeObject(jObject);
                    var outputBytes = Encoding.UTF8.GetBytes(output);
                    response.ContentLength = outputBytes.Length;
                    await response.Body.WriteAsync(outputBytes);
                }
                finally
                {
                    response.Body = originalBodyStream;
                }
            }
        }
    }

    /// <summary>
    /// Sử dụng HateoasMiddleware
    /// </summary>
    public static class HateoasMiddlewareExtensions
    {
        /// <summary>
        /// Thêm các link vào các response trả về
        /// </summary>
        /// <param name="builder">Đối tượng IApplicationBuilder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseHateoasMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<HateoasMiddleware>();
    }
}
