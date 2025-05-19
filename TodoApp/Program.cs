using api.v1.Services;
using api.v1.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Diagnostics;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http;

//Load .env as early as possible
DotNetEnv.Env.Load();

Console.WriteLine("Started!");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.Use(async (context, next) =>
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    Console.WriteLine($"Request: {context.Request.Method} {context.Response.StatusCode} {context.Request.Path} {stopwatch.ElapsedMilliseconds}ms");
});

app.Use(async (context, next) => {
    if (context.Request.RouteValues.TryGetValue("id", out var id)) {
        string? idString = id?.ToString();

        if (!ObjectId.TryParse(idString, out _)) {
            await context.Response.WriteAsync($"{idString} is not a valid 24 hex string");
            return;
        }
    }

    await next();
});

app.MapGet("/", () => "visit prefeix /api/v1/todos for api content");
// app.MapRazorPages();
app.MapTodoRoutes();

app.Run();