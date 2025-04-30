using System.Diagnostics;
using api.v1.Services;
using api.v1.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.Use(Logger.LogRequestAsync);

var v1 = app.MapGroup("/api/v1");

v1.MapGet("/", () => "Hello Wold!");
v1.MapGet("/get-todos", (ITodoService service) => service.GetTodos());
v1.MapGet("/get-todo/{id}", (int id) => {
    return "gerk: " + id;
});

app.Run();