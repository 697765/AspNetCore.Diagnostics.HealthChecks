using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.Slack
{
    public class SlackHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SlackHealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(SlackHealthCheckExtensions.NAME);

                var response = await httpClient.GetAsync("https://status.slack.com/api/v2.0.0/current").ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var json = JsonSerializer.Deserialize<SlackResponseStatus>(responseBody);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus,
                        $"Get status of Slack is not responding with 200 OK, the current status is {response.StatusCode}");
                }
                else if (json.status != "ok")
                {
                    return HealthCheckResult.Unhealthy();
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }

    public class SlackResponseStatus
    {
        public string status { get; set; }
    }
}
