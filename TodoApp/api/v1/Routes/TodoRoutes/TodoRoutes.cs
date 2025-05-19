using api.v1.Models;
using api.v1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MiniValidation;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Tests")]

namespace api.v1.Routes;

public static class TodoRoutes{

    //Extension method to all it to be callable by the "WebApplication" class, which is what the "app" object is 
    //and instance of.
    public static void MapTodoRoutes(this IEndpointRouteBuilder app){
        RouteGroupBuilder todoRoutes = app.MapGroup("/api/v1/todos");

        //GET routes.
        todoRoutes.MapGet("/get-todos", GetTodosHandler);
        todoRoutes.MapGet("/get-completed-todos", GetCompletedTodosHandler);
        todoRoutes.MapGet("/get-incompleted-todos", GetIncompletedTodosHandler);
        todoRoutes.MapGet("/get-todo/{id}", GetTodoByIdHandler);

        //POST route.
        todoRoutes.MapPost("/add-todo", AddTodoHandler);

        //PATCH routes
        todoRoutes.MapPatch("/update-todo-complete-status/{id}", UpdateTodoByidHandler);
        todoRoutes.MapPatch("/update-todoName/{id}", UpdateTodoByNameHandler);

        //Delete Route
        todoRoutes.MapDelete("/delete-todo/{id}", DeleteTodoByIdHandler);
    }

    //Handler to receive all todos from Todo service. 
    internal static async Task<IResult> GetTodosHandler(ITodoService service){
        List<Todo> todos = await service.GetTodosAsync();

        return Results.Json(todos, statusCode: 200);
    }

    //Handler to receive all Completed todos from Todo service.
    internal static async Task<IResult> GetCompletedTodosHandler(ITodoService service){
        ServiceResult<List<Todo>> todosResult = await service.GetCompletedTodosAsync();

        return Results.Json(todosResult.Data, statusCode: 200);
    }

    //Handler to receive all incomplete todos from Service.
    internal static async Task<IResult> GetIncompletedTodosHandler(ITodoService service){
        ServiceResult<List<Todo>> todosResult = await service.GetIncompletedTodosAsync();

        return Results.Json(todosResult.Data, statusCode: todosResult.StatusCode);
    }

    //Handler to receive one todo by id from service.
    internal static async Task<IResult> GetTodoByIdHandler(ITodoService service, string id){
        ServiceResult<Todo> getResult = await service.GetTodoByIdAsync(id);
        
        return Results.Json(getResult.Data, statusCode: getResult.StatusCode);
    }

    //Handler to add Todo to list of Todos.
    internal static async Task<IResult> AddTodoHandler(ITodoService service, [FromBody] Todo? todo) {    
        if (todo == null){
            return Results.BadRequest("Request body required!");
        }

        bool isValid = MiniValidator.TryValidate(todo, out var errors);

        //If the model validation failed, return the error messages to the client.
        if (!isValid){
            return Results.BadRequest(errors);
        }

        //Try adding the validated Todo to the list.
        ServiceResult<Todo> addTodoResult = await service.AddTodoAsync(todo);

        //Send back the result
        return Results.Json(addTodoResult, statusCode: addTodoResult.StatusCode);
    }

    //Handler to update Todo by id.
    internal static async Task<IResult> UpdateTodoByidHandler(ITodoService service, string id) {
        ServiceResult<UpdateResult> updateResult = await service.UpdateTodoByIdAsync(id);

        //Otherwise, return a success message!
        return Results.Json(updateResult, statusCode: updateResult.StatusCode);
    }

    //Handler to update the name of a Todo.
    internal static async Task<IResult> UpdateTodoByNameHandler(ITodoService service, string id, [FromBody] Todo? todo) {
        if (todo == null){
            return Results.BadRequest("Request body is required!");
        }

        bool isValid = MiniValidator.TryValidate(todo, out var errors);

        //If the name of todo is NOT valid, return the list of errors back to the client.
        if (!isValid){
            return Results.BadRequest(errors);
        }

        //Afterwards, call the service to change the todos name.
        ServiceResult<UpdateResult> updateResult = await service.UpdateTodoByNameAsync(id, todo);

        //If both the validation AND service are successful, return a success message!
        return Results.Json(updateResult, statusCode: updateResult.StatusCode);
    }

    //Handler to delete a todo by id.
    internal static async Task<IResult> DeleteTodoByIdHandler(ITodoService service, string id)  {
        ServiceResult<Todo> deleteTodoResult = await service.DeleteTodoByIdAsync(id);
        
        //Otherwise, return a success!
        return Results.Json(deleteTodoResult, statusCode: deleteTodoResult.StatusCode);
    }
}