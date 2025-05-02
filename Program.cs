using api.v1.Services;
using api.v1.Middlewares;
using api.v1.Models;

var builder = WebApplication.CreateBuilder(args);

//'AddSingleton' ensures that the TodoService instance is shared for all routes, and persists as long as the server is
//running. NOTE: this is just temporary until I get database functionality up and running. Afterwards, I will switch
//back to AddScoped.
builder.Services.AddSingleton<ITodoService, TodoService>();

var app = builder.Build();

app.Use(Logger.LogRequestAsync);

RouteGroupBuilder v1 = app.MapGroup("/api/v1");

v1.MapGet("/", () => "Hello World!");
v1.MapGet("/get-todos", (ITodoService service) => Results.Ok(service.GetTodos()));
v1.MapGet("/get-completed-todos", (ITodoService service) => {
    var todosResult = service.GetCompletedTodos();

    if (todosResult.Data == null){
        return Results.NotFound(todosResult);
    }

    return Results.Ok(todosResult.Data);
});

v1.MapGet("/get-incompleted-todos", (ITodoService service) => {
    var todosResult = service.GetIncompletedTodos();

    if (todosResult.Data == null){
        return Results.NotFound(todosResult);
    }

    return Results.Ok(todosResult.Data);
});

v1.MapGet("/get-todo/{id}", (ITodoService service, int id) => {
   ServiceResult<Todo> getResult = service.GetTodoById(id);
   
    if (getResult.Data == null){
        return Results.NotFound(getResult);
    }
    
    return Results.Ok(service.GetTodoById(id).Data);
});

v1.MapPost("/add-todo", (ITodoService service, Todo todo, HttpContext context) => {
    Console.WriteLine("todo: " + todo);
    
    var addTodoResult = service.AddTodo(todo);

    if (addTodoResult.Data == null){
       return Results.Conflict(addTodoResult);
    }

    return Results.Created(context.Request.Path, addTodoResult);
});

app.Run();