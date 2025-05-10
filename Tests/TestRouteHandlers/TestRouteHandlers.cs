using Xunit;
using api.v1.Services;
using api.v1.Routes;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using api.v1.Models;

public class TestTodoRouteHandlers{
    
    [Fact]
    public void TestGetTodosHandler(){
        ITodoService service = new TodoService();
        IResult result = TodoRoutes.GetTodosHandler(service);

        //Ensures that the GetTodosHandler returns a list of todos.
        Assert.IsType<Ok<List<Todo>>>(result);
    }

    [Fact]
    //Test to see if the GetCompletedTodosHandler() returns all todos that are completed. 
    //EXPECTED: List of todos containing Todos that are completed.
    public void TestGetCompletedTodosHandler_WithAllComplete(){
        ITodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, true),
            new Todo("todo 2", 2, true),
            new Todo("todo 3", 3, false),
        });
        var result = TodoRoutes.GetCompletedTodosHandler(service);

        Assert.IsType<Ok<List<Todo>>>(result);

        var okResult = result as Ok<List<Todo>>;

        Assert.NotNull(okResult);
        Assert.NotNull(okResult.Value);
        Assert.Equal(2, okResult.Value.Count);
    }

     [Fact]
    public void TestGetCompletedTodosHandler_WithAllIncomplete(){
        ITodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, false),
            new Todo("todo 3", 3, false),
        });
        var result = TodoRoutes.GetCompletedTodosHandler(service);

        Assert.IsType<NotFound<ServiceResult<List<Todo>>>>(result);
    }

    [Fact]
    //Test to see if the GetIncompletedTodosHandler() returns all todos that are incomplete. 
    //EXPECTED: List of todos containing Todos that are incompleted, with HttpResults type (ok).
    public void TestGetIncompletedTodosHandler_WithAllIncomplete(){
        ITodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, true),
            new Todo("todo 3", 3, false),
        });
        var result = TodoRoutes.GetIncompletedTodosHandler(service);

        Assert.IsType<Ok<List<Todo>>>(result);

        var okResult = result as Ok<List<Todo>>;

        Assert.NotNull(okResult);
        Assert.NotNull(okResult.Value);
        Assert.Equal(2, okResult.Value.Count);
    }

    [Fact]
    //Test to see if the GetIncompletedTodosHandler() returns a 404 when all todos that are completed. 
    //EXPECTED: Null, with a HttpResults type (NotFound).
    public void TestGetIncompletedTodosHandler_WithAllComplete(){
        ITodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, false),
            new Todo("todo 3", 3, false),
        });
        var result = TodoRoutes.GetCompletedTodosHandler(service);

        Assert.IsType<NotFound<ServiceResult<List<Todo>>>>(result);
    }

    [Fact]
    //Test to see if the GetTodoByIdHandler() returns a valid todo with the right id.
    //EXPECTED: valid todo, Ok<> response with a ServiceResult<Todo>
    public void TestGetTodoByIdHandler_WithValidId(){
        ITodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, false),
            new Todo("todo 3", 3, false),
        });
        var result = TodoRoutes.GetTodoByIdHandler(service, 2);

        //This is what should be return by the handler
        Assert.IsType<Ok<Todo>>(result);

        var okResult = result as Ok<Todo>;

        //Ensure that the ok result is not null.
        Assert.NotNull(okResult);

        //Ensure the ServiceResult object is not null.
        Assert.NotNull(okResult.Value);

        //Finally, ensure that the return data for the ServiceResult is not null.
        Assert.NotNull(okResult.Value);

        //Afterwards, enure that the retrieved todo is the same as the one in the list.
        Assert.Equal(new Todo("todo 2", 2, false), okResult.Value);
    }

    [Fact]
    //Test to see if the GetTodoByIdHandler() returns a NotFound response iwth null data todo.
    //EXPECTED: NotFound<> response with null
    public void TestGetTodoByIdHandler_WithInvalidId(){
        ITodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, false),
            new Todo("todo 3", 3, false),
        });
        var result = TodoRoutes.GetTodoByIdHandler(service, 22);

        //This is what should be return by the handler
        Assert.IsType<NotFound<ServiceResult<Todo>>>(result);
    }

    [Fact]
    //Test to see if the AddTodoHandler() returns a ok response after getting a valid todo.
    //EXPECTED: ok<> response with ServiceResult
    public void TestAddTodoByIdHandler_WithValidTodo(){
        ITodoService service = new TodoService();
        Todo todo = new Todo("todo to add", 22, false);
        HttpContext context = new DefaultHttpContext();
        var result = TodoRoutes.AddTodoHandler(service, todo, context);

        //This is what should be return by the handler
        Assert.IsType<Created<ServiceResult<Todo>>>(result);
    }
}