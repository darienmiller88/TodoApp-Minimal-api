using api.v1.Models;
using api.v1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();


app.MapGet("/", () => "Hello Wold!");
app.MapGet("/get-todos", () => todos);
app.MapGet("/get-todo/{id}", (int id) => {
    var todo = todos.FirstOrDefault(todo => todo.id == id);

    if (todo == null) {
        return Results.NotFound($"Todo with id: {id} not found.");
    }

    return Results.Ok(todo);
});

app.Run();