using api.v1.Services;
using api.v1.Middlewares;
using api.v1.Models;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

//'AddSingleton' ensures that the TodoService instance is shared for all routes, and persists as long as the server is
//running. NOTE: this is just temporary until I get database functionality up and running. Afterwards, I will switch
//back to AddScoped.
builder.Services.AddSingleton<ITodoService, TodoService>();

var app = builder.Build();

app.Use(Logger.LogRequestAsync);

app.MapGet("/", () => "visit prefeix /api/v1/ for api content");

RouteGroupBuilder v1 = app.MapGroup("/api/v1");

v1.MapDelete("/delete-todo/{id}", (ITodoService service, int id) => {
    ServiceResult<Todo> deleteTodoResult = service.DeleteTodoById(id);

    if (deleteTodoResult.Data == null){
        return Results.NotFound(deleteTodoResult);
    }

    return Results.Ok(deleteTodoResult);
});

v1.MapPatch("/update-todoName/{id}", (ITodoService service, int id, Todo todo) => {
    // ServiceResult<Todo> updateResult = service.UpdateTodoById(id);

    // if (updateResult.Data == null){
    //     return Results.NotFound(updateResult);
    // }
   
    return Results.Ok();
});

app.Run();