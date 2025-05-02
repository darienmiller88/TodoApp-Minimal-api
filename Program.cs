using api.v1.Services;
using api.v1.Middlewares;
using api.v1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.Use(Logger.LogRequestAsync);

RouteGroupBuilder v1 = app.MapGroup("/api/v1");

v1.MapGet("/", () => "Hello World!");
v1.MapGet("/get-todos", (ITodoService service) => Results.Ok(service.GetTodos()));
v1.MapGet("/get-todo/{id}", (ITodoService service, int id) => Results.Ok(service.GetTodoById(id)));
v1.MapPost("/add-todo", (ITodoService service, Todo todo) => {
    Console.WriteLine("todo: " + todo);
    
    var addTodoResult = service.AddTodo(todo);

    if (addTodoResult.IsSucess()){
        Results.Conflict(addTodoResult);
    }

    return Results.Ok(addTodoResult.Data);
});

app.Run();