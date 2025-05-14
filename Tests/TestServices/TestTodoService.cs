using Xunit;
using api.v1.Services;
using System.Collections.Generic;
using api.v1.Models;
using System.Threading.Tasks;
using Moq;
using MongoDB.Driver;

public class TestTodoService{

    [Fact]
    //Test to see if Service can get all todos from list.
    public async Task TestGetTodos(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService(mockCollection.Object);
        List<Todo> todos = await service.GetTodosAsync();

        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
    }

    [Fact]
    //Test to see if all completed todos are returned.
    public async Task TestGetCompletedTodos(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<List<Todo>> result = await service.GetCompletedTodosAsync();

        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if all incompleted todos are returned. It SHOULD NOT be null.
    public async Task TestGetIncompletedTodos(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<List<Todo>> result = await service.GetIncompletedTodosAsync();

        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if all if any todos are returned by GetCompletedTodos() when none are completed. it SHOULD be null.
    public async Task TestGetCompletedTodos_NoCompletedTodos(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<List<Todo>> result = await service.GetCompletedTodosAsync();

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    //Test to see if all if any todos are returned by GetIncompletedTodos() when all are completed. It SHOULD be null.
    public async Task TestGetIncompletedTodos_NoIncompletedTodos(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<List<Todo>> result = await service.GetIncompletedTodosAsync();

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    //Test to see if a todo with a certain id is returned succesfully. Result should NOT be null, and status code should
    //be 200 to reflect success.
    //EXPECTED: NOT NULL
    //STATUS CODE: 200
    public async Task TestGetTodoById(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService();
        ServiceResult<Todo> result = await service.GetTodoByIdAsync("frcfr");

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if a todo with a non-existent id returns null. 
    //EXPECTED: NULL
    //STATUS CODE: 404
    public async Task TestGetTodoById_InvalidId(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService();
        ServiceResult<Todo> result = await service.GetTodoByIdAsync("11111");

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    //Test to see if a adding a valid todo works.
    //EXPECTED: Todo (Not Null)
    //STATUS CODE: 201
    public async Task TestAddValidTodo(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService();
        ServiceResult<Todo> result = await service.AddTodoAsync(new Todo("example todo", false));

        Assert.NotNull(result.Data);
        Assert.Equal(201, result.StatusCode);
    }

    [Fact]
    //Test to see if adding an invalid todo with a dupliucate id returns null and 409.
    //EXPECTED: Null
    //STATUS CODE: 409
    public async Task TestAddInvalid_TodoDuplicateId(){
        TodoService service = new TodoService();
        ServiceResult<Todo> result = await service.AddTodoAsync(new Todo("example todo", false));

        //First todo is valid, should not be null, and should return 200
        Assert.NotNull(result.Data);
        Assert.Equal(201, result.StatusCode);

        //Add a new todo with the same id as the above one.
        Todo invalidTodo = new Todo("example todo again", false);
        result = await service.AddTodoAsync(invalidTodo);
        
        //Newly added todo should be null, and should return 409
        Assert.Null(result.Data);
        Assert.Equal(409, result.StatusCode);

        //This error message should be returned when an invalid todo is received by the AddTodo service.
        Assert.Equal($"Todo with id of \'{invalidTodo.Id}\' already exists!", result.Message);
    }

    [Fact]
    //Test to see if a adding a invalid todo with a duplicate name return null and 409
    //EXPECTED: Null
    //STATUS CODE: 409
    public async Task TestAddInvalidTodoDuplicateName(){
        TodoService service = new TodoService();
        ServiceResult<Todo> result = service.AddTodo(new Todo("example todo", 5, false));

        //Add a valid todo first.
        Assert.NotNull(result.Data);
        Assert.Equal(201, result.StatusCode);

        //Add a todo with a duplicate name.
        Todo invalidTodo = new Todo("example todo", 15, false);
        result = service.AddTodo(invalidTodo);

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
        TodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, true),
        });
        ServiceResult<Todo> result = service.DeleteTodoById(1);

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
    }
    
    [Fact]
    //Test to see if a deleting an invalid todo returns null.
    //EXPECTED: Null
    //STATUS CODE: 404
    public async Task TestDeleteTodoByInvalidId(){
        TodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, true),
        });
        int id = 11;
        ServiceResult<Todo> result = service.DeleteTodoById(id);

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No todo with id {id} found!", result.Message);
    }

     [Fact]
    //Test to see if a updating a valid todo works.
    //EXPECTED: ServiceResult (Not Null)
    //STATUS CODE: 200
    public async Task TestUpdateValidTodoById(){
        TodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, true),
        });
        ServiceResult<Todo> result = service.UpdateTodoById(1);

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if a updating an invalid todo returns null.
    //EXPECTED: Null
    //STATUS CODE: 404
    public async Task TestUpdateTodoByInvalidId(){
        TodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, true),
        });
        int id = 11;
        ServiceResult<Todo> result = service.UpdateTodoById(id);

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No todo with id {id} found!", result.Message);
    }

    [Fact]
    //Test to see if a updating a todo with a new name works properly
    //EXPECTED: ServiceResult 
    //STATUS CODE: 200
    public async Task TestUpdateTodoByName(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService();
        ServiceResult<UpdateResult> result = await service.UpdateTodoByNameAsync("f", new Todo("new todo name", false));

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if a updating a todo with a new name but invalid id returns null.
    //EXPECTED: null 
    //STATUS CODE: 404
    public async Task TestUpdateInvalidTodoByName(){
        TodoService service = new TodoService(new List<Todo>{
            new Todo("todo1", 1, false),
            new Todo("todo1", 2, true),
        });
        int id = 11;
        ServiceResult<Todo> result = service.UpdateTodoByName(id, new Todo("new todo name", 5, false));

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No todo with id {id} found!", result.Message);
    }
}