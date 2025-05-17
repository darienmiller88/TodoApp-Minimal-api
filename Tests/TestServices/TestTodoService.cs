using Xunit;
using api.v1.Services;
using System.Collections.Generic;
using api.v1.Models;
using System.Threading.Tasks;
using Moq;
using MongoDB.Driver;
using System.Threading;
using System.Linq;

namespace Tests;
public class TestTodoService{

    [Fact]
    //Test to see if Service can get all todos from list.
    public async Task TestGetTodos(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("go home", false),
            new Todo("finish work", true),
        });
        TodoService service = new TodoService(mockCollection.Object);
        List<Todo> todos = await service.GetTodosAsync();

        Assert.NotNull(todos);  
        Assert.NotEmpty(todos);
        Assert.Equal(2, todos.Count);
    }

    [Fact]
    //Test to see if all completed todos are returned.
    public async Task TestGetCompletedTodos(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("todo 1", true),
            new Todo("todo 2", true)
        });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<List<Todo>> result = await service.GetCompletedTodosAsync();

        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2,result.Data.Count);
    }

    [Fact]
    //Test to see if all incompleted todos are returned. It SHOULD NOT be null.
    public async Task TestGetIncompletedTodos(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("todo 1", false),
            new Todo("todo 2", false)
        });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<List<Todo>> result = await service.GetIncompletedTodosAsync();

        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2,result.Data.Count);
    }

    [Fact]
    //Test to see if all if any todos are returned by GetCompletedTodos() when none are completed. The array should be EMPTY.
    public async Task TestGetCompletedTodos_NoCompletedTodos(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("todo 1", false),
            new Todo("todo 2", false)
        });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<List<Todo>> result = await service.GetCompletedTodosAsync();

        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    //Test to see if all if any todos are returned by GetIncompletedTodos() when all are completed. It SHOULD be null.
    public async Task TestGetIncompletedTodos_NoIncompletedTodos(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{
            new Todo("todo 1", true),
            new Todo("todo 2", true)
        });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<List<Todo>> result = await service.GetIncompletedTodosAsync();

        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    //Test to see if a todo with a certain id is returned succesfully. Result should NOT be null, and status code should
    //be 200 to reflect success.
    //EXPECTED: NOT NULL
    //STATUS CODE: 200
    public async Task TestGetTodoById(){
        Todo t1 = new Todo("todo 1", false);
        Todo t2 = new Todo("todo 2", false);

        //Assign object ids first.
        t1.AssignRandomMongoObjectId();
        t2.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1, t2 });
        TodoService service = new TodoService(mockCollection.Object);
        List<Todo> todos = await service.GetTodosAsync();

        Assert.NotNull(todos);
        Assert.Equal(2, todos.Count);

        ServiceResult<Todo> resultT1 = await service.GetTodoByIdAsync(t1.Id);
        ServiceResult<Todo> resultT2 = await service.GetTodoByIdAsync(t2.Id);

        //Check to see if the original todos, and the ones we retrieved by their ids
        Assert.Equal(t1, resultT1.Data);
        Assert.Equal(t2, resultT2.Data);
    }

    [Fact]
    //Test to see if a todo with a non-existent id returns null. 
    //EXPECTED: NULL
    //STATUS CODE: 404
    public async Task TestGetTodoById_InvalidId(){
        Todo t1 = new Todo("todo 1", false);
        Todo t2 = new Todo("todo 2", false);

        //Assign object ids first.
        t1.AssignRandomMongoObjectId();
        t2.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1, t2 });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<Todo> result = await service.GetTodoByIdAsync("bhjbbui");//Check for empty

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);

        result = await service.GetTodoByIdAsync("FH3UIJORE");//Check for fake id

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    //Test to see if a adding a valid todo works.
    //EXPECTED: Todo (Not Null)
    //STATUS CODE: 201
    public async Task TestAddValidTodo(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{});
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<Todo> result = await service.AddTodoAsync(new Todo("example todo", false));

        Assert.NotNull(result.Data);
        Assert.Equal(201, result.StatusCode);

        List<Todo> todos = await service.GetTodosAsync();

        //Assert that only one todo is in the fake database.
        Assert.Single(todos);
    }

    [Fact]
    //Test to see if a adding a invalid todo with a duplicate name return null and 409
    //EXPECTED: Null
    //STATUS CODE: 409
    public async Task TestAddInvalidTodo_DuplicateName(){
        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{});
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<Todo> result = await service.AddTodoAsync(new Todo("example todo", false));

        //Add a valid todo first.
        Assert.NotNull(result.Data);
        Assert.Equal(201, result.StatusCode);

        //Add a todo with a duplicate name.
        Todo invalidTodo = new Todo("example todo", false);
        result = await service.AddTodoAsync(invalidTodo);

        //Newly added todo should be null, return the proper error message and should return 409
        Assert.Null(result.Data);
        Assert.Equal(409, result.StatusCode);
        Assert.Equal($"Todo with name of \'{invalidTodo.TodoName}\' already exists!", result.Message);
    }

    [Fact]
    //Test to see if a deleting a valid todo works.
    //EXPECTED: ServiceResult (Not Null)
    //STATUS CODE: 200
    public async Task TestDeleteValidTodoById(){
        Todo t1 = new Todo("new todo", false);

        t1.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1 });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<Todo> result = await service.DeleteTodoByIdAsync(t1.Id);

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
        
        List<Todo> todos = await service.GetTodosAsync();
        
        //Check to see if the todos is empty after removing the "t1" todo we added above
        Assert.Empty(todos);
    }
    
    [Fact]
    //Test to see if a deleting an invalid todo returns null.
    //EXPECTED: Null
    //STATUS CODE: 404
    public async Task TestDeleteTodoByInvalidId(){
        Todo t1 = new Todo("new todo", false);

        t1.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1 });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<Todo> result = await service.DeleteTodoByIdAsync("ifcbeucd");

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No todo with id {"ifcbeucd"} found!", result.Message);
    }

     [Fact]
    //Test to see if a updating a valid todo works.
    //EXPECTED: ServiceResult (Not Null)
    //STATUS CODE: 200
    public async Task TestUpdateValidTodoById(){
        Todo t1 = new Todo("new todo", false);

        t1.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1 });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<UpdateResult> result = await service.UpdateTodoByIdAsync(t1.Id);

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if a updating a todo by an invalid id returns null.
    //EXPECTED: Null
    //STATUS CODE: 404
    public async Task TestUpdateTodo_ByInvalidId(){
        Todo t1 = new Todo("new todo", false);

        t1.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1 });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<UpdateResult> result = await service.UpdateTodoByIdAsync("1");

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No todo with id {"1"} found!", result.Message);
    }

    [Fact]
    //Test to see if a updating a todo with a new name works properly
    //EXPECTED: ServiceResult 
    //STATUS CODE: 200
    public async Task TestUpdateTodoByName(){
        Todo t1 = new Todo("new todo", false);

        t1.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1 });
        TodoService service = new TodoService(mockCollection.Object);
        string newTodoName = "go home";
        ServiceResult<UpdateResult> result = await service.UpdateTodoByNameAsync(t1.Id, new Todo(newTodoName, false));

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);

        //Get all of the todos after updating...
        List<Todo> todos = await service.GetTodosAsync();
    
        //And check to see if it has the same name as the new name we updated it with
        Assert.Equal(newTodoName, todos.ElementAt(0).TodoName);
    }

    [Fact]
    //Test to see if a updating a todo with a new name but invalid id returns null.
    //EXPECTED: null 
    //STATUS CODE: 404
    public async Task TestUpdateTodoByName_InvalidId(){
         Todo t1 = new Todo("new todo", false);

        t1.AssignRandomMongoObjectId();

        var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{ t1 });
        TodoService service = new TodoService(mockCollection.Object);
        string id = "fake_id";
        ServiceResult<UpdateResult> result = await service.UpdateTodoByNameAsync(id, new Todo("new one", false));

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No todo with id {id} found!", result.Message);
    }
}