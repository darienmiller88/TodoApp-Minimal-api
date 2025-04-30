using System.Diagnostics;

namespace api.v1.Middlewares;

static class Logger{
    public static async Task LogRequestAsync(HttpContext context, Func<Task> next) {
        ILogger logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("RequestLogger");
        Stopwatch stopwatch = Stopwatch.StartNew();

        await next();

        stopwatch.Stop();

        logger.LogInformation("HTTP {method} {path} responded {statusCode} in {elapsed}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds
        );
    }
};