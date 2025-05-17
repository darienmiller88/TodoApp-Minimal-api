using api.v1.Services;
using api.v1.Middlewares;
using api.v1.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

//Load .env as early as possible
DotNetEnv.Env.Load();

Console.WriteLine("mongo uri : " + Environment.GetEnvironmentVariable("MONGO_URI"));
Console.WriteLine("Started!");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.Use(Logger.LogRequestAsync);

app.MapGet("/", () => "visit prefeix /api/v1/todos for api content");
// app.MapRazorPages();
app.MapTodoRoutes();

app.Run();