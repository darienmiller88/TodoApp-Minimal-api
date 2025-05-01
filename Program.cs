using api.v1.Services;
using api.v1.Middlewares;
using api.v1.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.Use(Logger.LogRequestAsync);

RouteGroupBuilder v1 = app.MapGroup("/api/v1");

v1.MapGet("/", () => "Hello World!");
v1.MapGet("/get-todos", (ITodoService service) => service.GetTodos());
v1.MapGet("/get-todo/{id}", (ITodoService service, int id) => service.GetTodoById(id));
v1.MapPost("/add-todo", (Todo todo) => {
    Console.WriteLine("todo: " + JsonSerializer.Serialize(todo));

    return Results.Ok(todo);
});

app.Run();