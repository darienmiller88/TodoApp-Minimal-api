using api.v1.Models;
using api.v1.Services;
namespace api.v1.Routes;


public static class TodoRoutes{
    public static void MapTodoRoutes(this IEndpointRouteBuilder app){
        RouteGroupBuilder todoRoutes = app.MapGroup("/api/v1/todos");

        todoRoutes.MapGet("/get-todos", (ITodoService service) => Results.Ok(service.GetTodos()));
        todoRoutes.MapGet("/get-completed-todos", (ITodoService service) => {
            var todosResult = service.GetCompletedTodos();

            if (todosResult.Data == null){
                return Results.NotFound(todosResult);
            }
            
            return Results.Ok(todosResult.Data);
        });

        todoRoutes.MapGet("/get-incompleted-todos", (ITodoService service) => {
            var todosResult = service.GetIncompletedTodos();

            if (todosResult.Data == null){
                return Results.NotFound(todosResult);
            }

            return Results.Ok(todosResult.Data);
        });

        todoRoutes.MapGet("/get-todo/{id}", (ITodoService service, int id) => {
            ServiceResult<Todo> getResult = service.GetTodoById(id);
        
            if (getResult.Data == null){
                return Results.NotFound(getResult);
            }
            
            return Results.Ok(service.GetTodoById(id).Data);
        });
    }
}