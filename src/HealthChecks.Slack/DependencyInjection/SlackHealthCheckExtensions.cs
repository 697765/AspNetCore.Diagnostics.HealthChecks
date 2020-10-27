using HealthChecks.Slack;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SlackHealthCheckExtensions
    {
        internal const string NAME = "slack";

        /// <summary>
        /// Add a health check for Slack.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
      
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'slack' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddSlack(this IHealthChecksBuilder builder, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
        {
            var registrationName = name ?? NAME;

            builder.Services.AddHttpClient(registrationName);

            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => new SlackHealthCheck(sp.GetRequiredService<IHttpClientFactory>()),
                failureStatus,
                tags,
                timeout));
        }
    }
}
