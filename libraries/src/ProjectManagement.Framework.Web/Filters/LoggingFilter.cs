using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ProjectManagement.Framework.Web.Filters;

public class LoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<LoggingFilter> _logger;

    public LoggingFilter(ILogger<LoggingFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _logger.LogDebug("Endpoint call start: {endpoint}", context.ActionDescriptor.DisplayName);

        Stopwatch sw = Stopwatch.StartNew();
        await next();
        sw.Stop();

        _logger.LogDebug("Endpoint call end: {endpoint} taking {duration}ms", context.ActionDescriptor.DisplayName,
            sw.ElapsedMilliseconds.ToString());
    }
}