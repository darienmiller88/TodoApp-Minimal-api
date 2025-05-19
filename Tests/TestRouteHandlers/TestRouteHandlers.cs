using Xunit;
using api.v1.Services;
using api.v1.Routes;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using api.v1.Models;
using System.Threading.Tasks;
using Tests;

public class TestTodoRouteHandlers{
    
    [Fact]
    public async Task TestGetTodosHandler(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("todo1", false),
            new Todo("todo2", true)
        });
        ITodoService service = new TodoService(mockCollection.Object);
        IResult result = await TodoRoutes.GetTodosHandler(service);

        //Ensures that the GetTodosHandler returns a list of todos.
        Assert.IsType<List<Todo>>(result);
    }

    [Fact]
    //Test to see if the GetCompletedTodosHandler() returns all todos that are completed. 
    //EXPECTED: List of todos containing Todos that are completed.
    public async Task TestGetCompletedTodosHandler_WithAllComplete(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("todo1", false),
            new Todo("todo2", true)
        });
        ITodoService service = new TodoService(mockCollection.Object);
        var result = await TodoRoutes.GetCompletedTodosHandler(service);

        Assert.IsType<List<Todo>>(result);

        var okResult = result as List<Todo>;

        Assert.NotNull(okResult);
        Assert.Equal(2, okResult.Count);
    }

     [Fact]
    public async Task TestGetCompletedTodosHandler_WithAllIncomplete(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("todo1", false),
            new Todo("todo2", false)
        });
        ITodoService service = new TodoService(mockCollection.Object);
        var result = await TodoRoutes.GetCompletedTodosHandler(service);

        Assert.IsType<NotFound<ServiceResult<List<Todo>>>>(result);
    }

    [Fact]
    //Test to see if the GetIncompletedTodosHandler() returns all todos that are incomplete. 
    //EXPECTED: List of todos containing Todos that are incompleted, with HttpResults type (ok).
    public async Task TestGetIncompletedTodosHandler_WithAllIncomplete(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("todo1", false),
            new Todo("todo2", false)
        });
        ITodoService service = new TodoService(mockCollection.Object);
        var result = await TodoRoutes.GetIncompletedTodosHandler(service);

        Assert.IsType<Ok<List<Todo>>>(result);

        var okResult = result as Ok<List<Todo>>;

        Assert.NotNull(okResult);
        Assert.NotNull(okResult.Value);
        Assert.Equal(2, okResult.Value.Count);
    }

    [Fact]
    //Test to see if the GetIncompletedTodosHandler() returns a 404 when all todos that are completed. 
    //EXPECTED: Null, with a HttpResults type (NotFound).
    public async Task TestGetIncompletedTodosHandler_WithAllComplete(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("todo1", true),
            new Todo("todo2", true)
        });
        ITodoService service = new TodoService(mockCollection.Object);
        var result = await TodoRoutes.GetCompletedTodosHandler(service);

        Assert.IsType<NotFound<ServiceResult<List<Todo>>>>(result);
    }

    [Fact]
    //Test to see if the GetTodoByIdHandler() returns a valid todo with the right id.
    //EXPECTED: valid todo, Ok<> response with a ServiceResult<Todo>
    public async Task TestGetTodoByIdHandler_WithValidId(){
        Todo t1 = new Todo("todo1", false);
        Todo t2 = new Todo("todo2", false);

        t1.AssignRandomMongoObjectId();
        t2.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1, t2 });
        ITodoService service = new TodoService(mockCollection.Object);
        var result = await TodoRoutes.GetTodoByIdHandler(service, t1.Id);

        //This is what should be return by the handler
        Assert.IsType<Ok<Todo>>(result);

        var okResult = result as Ok<Todo>;

        //Ensure that the ok result is not null.
        Assert.NotNull(okResult);

        //Ensure the ServiceResult object is not null.
        Assert.NotNull(okResult.Value);

        //Afterwards, enure that the retrieved todo is the same as the one in the list.
        Assert.Equal(t1, okResult.Value);
    }

    [Fact]
    //Test to see if the GetTodoByIdHandler() returns a NotFound response iwth null data todo.
    //EXPECTED: NotFound<> response with null
    public async Task TestGetTodoByIdHandler_WithInvalidId(){
        Todo t1 = new Todo("todo1", false);
        Todo t2 = new Todo("todo2", false);

        t1.AssignRandomMongoObjectId();
        t2.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1, t2 });
        ITodoService service = new TodoService(mockCollection.Object);
        var result = await TodoRoutes.GetTodoByIdHandler(service, "fake_id");

        //This is what should be return by the handler
        Assert.IsType<NotFound<ServiceResult<Todo>>>(result);
    }

    [Fact]
    //Test to see if the AddTodoHandler() returns a ok response after getting a valid todo.
    //EXPECTED: ok<> response with ServiceResult
    public async Task TestAddTodoByIdHandler_WithValidTodo(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{});
        ITodoService service = new TodoService(mockCollection.Object);
        Todo todo = new Todo("todo to add", false);
        var result = await TodoRoutes.AddTodoHandler(service, todo);

        //This is what should be return by the handler
        Assert.IsType<Created<ServiceResult<Todo>>>(result);
    }
}