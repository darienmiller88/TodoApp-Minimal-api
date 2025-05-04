using api.v1.Models;
using api.v1.Services;
namespace api.v1.Routes;


public static class TodoRoutes{
    public static void MapTodoRoutes(this IEndpointRouteBuilder app){
        RouteGroupBuilder todoRoutes = app.MapGroup("/api/v1/todos");

        //GET routes.
        todoRoutes.MapGet("/get-todos", GetTodosHandler);
        todoRoutes.MapGet("/get-completed-todos", GetCompletedTodosHandler);
        todoRoutes.MapGet("/get-incompleted-todos", GetIncompletedTodosHandler);
        todoRoutes.MapGet("/get-todo/{id}", GetTodoByIdHandler);
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

}