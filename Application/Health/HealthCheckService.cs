using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestSharp;

namespace Application.Health
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class HealthCheckService : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            const string URL = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random";
            var client = new RestClient();
            var request = new RestRequest(URL);
            request.AddHeader("accept", "application/json");
            request.AddHeader("X-RapidAPI-Key", "cd714ad54emshf8ffe1bec08a559p1204b8jsn9d3f1f0b8e00");
            request.AddHeader("X-RapidAPI-Host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com");
            var response = client.Execute(request, cancellationToken);
            return Task.FromResult(response.IsSuccessful 
                ? HealthCheckResult.Healthy("Chuck Norris is healthy") 
                : HealthCheckResult.Unhealthy("Chuck Norris is not healthy"));
        }
    }
}
