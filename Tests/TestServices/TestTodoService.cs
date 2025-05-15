using Xunit;
using api.v1.Services;
using System.Collections.Generic;
using api.v1.Models;
using System.Threading.Tasks;
using Moq;
using MongoDB.Driver;
using System.Threading;
using System.Linq;

public class TestTodoService{

    private Mock<IMongoCollection<Todo>> GetMockTodoService(List<Todo> dummyTodos){
        //First, create a new cursor object, which will be used by our fake mongo object to look up the mongo cluster
        var mockCursor = new Mock<IAsyncCursor<Todo>>();

        //Setup the cursor to return a list of todos as the dummy data.
        mockCursor.Setup(_ => _.Current).Returns(dummyTodos);
        
        //Finally, Set up the sequence so that at most, one batch of data is retrieved, and no more.
        mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);
    
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        mockCollection.Setup(x => x.FindAsync(
            It.IsAny<FilterDefinition<Todo>>(),
            It.IsAny<FindOptions<Todo, Todo>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(mockCursor.Object);

        return mockCollection;
    }

    [Fact]
    //Test to see if Service can get all todos from list.
    public async Task TestGetTodos(){
        var mockCollection = GetMockTodoService(new List<Todo>{
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
        var mockCollection = GetMockTodoService(new List<Todo>{
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
        var mockCollection = GetMockTodoService(new List<Todo>{
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
        var mockCollection = GetMockTodoService(new List<Todo>{
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
        var mockCollection = GetMockTodoService(new List<Todo>{
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

        var mockCollection = GetMockTodoService(new List<Todo>{ t1, t2 });
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

        var mockCollection = GetMockTodoService(new List<Todo>{ t1, t2 });
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<Todo> result = await service.GetTodoByIdAsync("");//Check for empty

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
        var mockCollection = GetMockTodoService(new List<Todo>{});
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<Todo> result = await service.AddTodoAsync(new Todo("example todo", false));

        Assert.NotNull(result.Data);
        Assert.Equal(201, result.StatusCode);

        List<Todo> todos = await service.GetTodosAsync();

        //Assert that only one todo is in the fake database.
        Assert.Single(todos);
    }

    // [Fact]
    // //Test to see if adding an invalid todo with a dupliucate id returns null and 409.
    // //EXPECTED: Null
    // //STATUS CODE: 409
    // public async Task TestAddInvalid_TodoDuplicateId(){
    //     var mockCollection = GetMockTodoService(new List<Todo>{});
    //     TodoService service = new TodoService(mockCollection.Object);
    //     ServiceResult<Todo> result = await service.AddTodoAsync(new Todo("example todo", false));

    //     //First todo is valid, should not be null, and should return 200
    //     Assert.NotNull(result.Data);
    //     Assert.Equal(201, result.StatusCode);

    //     //Add a new todo with the same id as the above one.
    //     Todo invalidTodo = new Todo("example todo again", false);
    //     result = await service.AddTodoAsync(invalidTodo);
        
    //     //Newly added todo should be null, and should return 409
    //     Assert.Null(result.Data);
    //     Assert.Equal(409, result.StatusCode);

    //     //This error message should be returned when an invalid todo is received by the AddTodo service.
    //     Assert.Equal($"Todo with id of \'{invalidTodo.Id}\' already exists!", result.Message);
    // }

    [Fact]
    //Test to see if a adding a invalid todo with a duplicate name return null and 409
    //EXPECTED: Null
    //STATUS CODE: 409
    public async Task TestAddInvalidTodo_DuplicateName(){
        var mockCollection = GetMockTodoService(new List<Todo>{});
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

        var mockCollection = GetMockTodoService(new List<Todo>{ t1 });
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

        var mockCollection = GetMockTodoService(new List<Todo>{ t1 });
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
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<UpdateResult> result = await service.UpdateTodoByIdAsync("gkv");

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if a updating an invalid todo returns null.
    //EXPECTED: Null
    //STATUS CODE: 404
    public async Task TestUpdateTodo_ByInvalidId(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<UpdateResult> result = await service.UpdateTodoByIdAsync("id");

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No todo with id  found!", result.Message);
    }

    [Fact]
    //Test to see if a updating a todo with a new name works properly
    //EXPECTED: ServiceResult 
    //STATUS CODE: 200
    public async Task TestUpdateTodoByName(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService(mockCollection.Object);
        ServiceResult<UpdateResult> result = await service.UpdateTodoByNameAsync("f", new Todo("new todo name", false));

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if a updating a todo with a new name but invalid id returns null.
    //EXPECTED: null 
    //STATUS CODE: 404
    public async Task TestUpdateInvalidTodoByName(){
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        TodoService service = new TodoService(mockCollection.Object);
        int id = 11;
        ServiceResult<UpdateResult> result = await service.UpdateTodoByNameAsync("id", new Todo("new todo name", false));

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No todo with id {id} found!", result.Message);
    }
}