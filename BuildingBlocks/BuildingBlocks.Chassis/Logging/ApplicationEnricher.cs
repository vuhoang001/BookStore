using System.Security.Claims;
using BuildingBlocks.Constants.Other;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.Enrichment;

namespace BuildingBlocks.Chassis.Logging;

public class ApplicationEnricher(IHttpContextAccessor httpContextAccessor) : ILogEnricher
{
    public void Enrich(IEnrichmentTagCollector collector)
    {
        collector.Add(LoggingConstant.MachineName, LoggingConstant.UserId);
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is not null)
        {
            collector.Add(LoggingConstant.UserId,
                          httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        }
    }
}
