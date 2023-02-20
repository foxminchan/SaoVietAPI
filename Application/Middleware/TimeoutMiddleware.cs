using Microsoft.AspNetCore.Http;

namespace Application.Middleware;

public class TimeoutMiddleware
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    private readonly RequestDelegate _next;

    public TimeoutMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        using var timeoutCts = new CancellationTokenSource();
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30), timeoutCts.Token);

        var responseTask = _next(context);

        var completedTask = await Task.WhenAny(responseTask, timeoutTask);
        if (completedTask == timeoutTask)
        {
            context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
            await context.Response.WriteAsync("Request timed out", cancellationToken: timeoutCts.Token);
            return;
        }

        timeoutCts.Cancel();

        await responseTask;
    }
}
