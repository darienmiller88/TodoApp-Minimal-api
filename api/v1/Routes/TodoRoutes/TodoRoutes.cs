using api.v1.Models;
using api.v1.Services;
using MiniValidation;
namespace api.v1.Routes;


public static class TodoRoutes{
    public static void MapTodoRoutes(this IEndpointRouteBuilder app){
        RouteGroupBuilder todoRoutes = app.MapGroup("/api/v1/todos");

        //GET routes.
        todoRoutes.MapGet("/get-todos", GetTodosHandler);
        todoRoutes.MapGet("/get-completed-todos", GetCompletedTodosHandler);
        todoRoutes.MapGet("/get-incompleted-todos", GetIncompletedTodosHandler);
        todoRoutes.MapGet("/get-todo/{id}", GetTodoByIdHandler);

        //POST route(s).
        todoRoutes.MapPost("/add-todo", AddTodoHandler);

        //PATCH routes
        todoRoutes.MapPatch("/update-todo-complete-status/{id}", UpdateTodoByid);
    }

    //Handler to receive all todos from Todo service.
    private static IResult GetTodosHandler(ITodoService service){
        return Results.Ok(service.GetTodos());
    }

    //Handler to receive all Completed todos from Todo service.
    private static IResult GetCompletedTodosHandler(ITodoService service){
        var todosResult = service.GetCompletedTodos();

        if (todosResult.Data == null){
            return Results.NotFound(todosResult);
        }
        
        return Results.Ok(todosResult.Data);
    }

    //Handler to receive all incomplete todos from Service.
    private static IResult GetIncompletedTodosHandler(ITodoService service){
        var todosResult = service.GetIncompletedTodos();

        if (todosResult.Data == null){
            return Results.NotFound(todosResult);
        }

        return Results.Ok(todosResult.Data);
    }

    //Handler to receive one todo by id from service.
    private static IResult GetTodoByIdHandler(ITodoService service, int id){
        ServiceResult<Todo> getResult = service.GetTodoById(id);
        
        if (getResult.Data == null){
            return Results.NotFound(getResult);
        }
        
        return Results.Ok(service.GetTodoById(id).Data);
    }

    //Handler to add Todo to list of Todos.
    private static IResult AddTodoHandler(ITodoService service, Todo todo, HttpContext context) {    
        bool isValid = MiniValidator.TryValidate(todo, out var errors);

        //If the model validation failed, return the error messages to the client.
        if (!isValid){
            return Results.BadRequest(errors);
        }

        //Try adding the validated Todo to the list.
        ServiceResult<Todo> addTodoResult = service.AddTodo(todo);

        //If that fails, send back the errors.
        if (addTodoResult.Data == null){
            return Results.Conflict(addTodoResult);
        }

        //Otherwise, send back a success!
        return Results.Created(context.Request.Path, addTodoResult);
    }

    private static IResult UpdateTodoByid(ITodoService service, int id) {
        ServiceResult<Todo> updateResult = service.UpdateTodoById(id);

        //If the id doesn't exist, return a 404.
        if (updateResult.Data == null){
            return Results.NotFound(updateResult);
        }

        //Otherwise, return a success message!
        return Results.Ok(updateResult);
    }

}