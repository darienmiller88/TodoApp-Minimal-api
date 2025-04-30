using System.Diagnostics;
using api.v1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.Use(async (context, next) => {
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
});

var v1 = app.MapGroup("/api/v1");

v1.MapGet("/", () => "Hello Wold!");
v1.MapGet("/get-todos", (ITodoService service) => service.GetTodos());
v1.MapGet("/get-todo/{id}", (int id) => {
    return "gerk: " + id;
});

app.Run();