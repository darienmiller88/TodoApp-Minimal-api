using Xunit;
using api.v1.Services;
using System.Collections.Generic;
using api.v1.Models;

public class TestTodoService{

    [Fact]
    //Test to see if Service can get all todos from list.
    public void TestGetTodos(){
        TodoService service = new TodoService();
        List<Todo> todos = service.GetTodos();

        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
    }

    [Fact]
    //Test to see if all completed todos are returned.
    public void TestGetCompletedTodos(){
        TodoService service = new TodoService();
        ServiceResult<List<Todo>> result = service.GetCompletedTodos();

        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if all incompleted todos are returned. It SHOULD NOT be null.
    public void TestGetIncompletedTodos(){
        TodoService service = new TodoService();
        ServiceResult<List<Todo>> result = service.GetIncompletedTodos();

        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if all if any todos are returned by GetCompletedTodos() when none are completed. it SHOULD be null.
    public void TestGetCompletedTodosWhenNoCompletedTodos(){
        TodoService service = new TodoService(new List<Todo>{
            new Todo("example", 1, false),
            new Todo("example1", 2, false),
            new Todo("example3", 3, false),
        });
        ServiceResult<List<Todo>> result = service.GetCompletedTodos();

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    //Test to see if all if any todos are returned by GetIncompletedTodos() when all are completed. It SHOULD be null.
    public void TestGetIncompletedTodosWhenNoIncompletedTodos(){
        TodoService service = new TodoService(new List<Todo>{
            new Todo("example", 1, true),
            new Todo("example1", 2, true),
            new Todo("example3", 3, true),
        });
        ServiceResult<List<Todo>> result = service.GetIncompletedTodos();

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    //Test to see if a todo with a certain id is returned succesfully. Result should NOT be null, and status code should
    //be 200 to reflect success.
    //EXPECTED: NOT NULL
    //STATUS CODE: 200
    public void TestGetTodoById(){
        TodoService service = new TodoService();
        ServiceResult<Todo> result = service.GetTodoById(1);

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    //Test to see if a todo with a non-existent id returns null. 
    //EXPECTED: NULL
    //STATUS CODE: 404
    public void TestGetTodoByIdWithInvalidId(){
        TodoService service = new TodoService();
        ServiceResult<Todo> result = service.GetTodoById(11111);

        Assert.Null(result.Data);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    //Test to see if a adding a valid todo works.
    //EXPECTED: Todo (Not Null)
    //STATUS CODE: 201
    public void TestAddValidTodo(){
        TodoService service = new TodoService();
        ServiceResult<Todo> result = service.AddTodo(new Todo("example todo", 5, false));

        Assert.NotNull(result.Data);
        Assert.Equal(201, result.StatusCode);
    }

    [Fact]
    //Test to see if adding an invalid todo with a dupliucate id returns null and 409.
    //EXPECTED: Null
    //STATUS CODE: 409
    public void TestAddInvalidTodoDuplicateId(){
        TodoService service = new TodoService();
        ServiceResult<Todo> result = service.AddTodo(new Todo("example todo", 5, false));

        //First todo is valid, should not be null, and should return 200
        Assert.NotNull(result.Data);
        Assert.Equal(201, result.StatusCode);

        //Add a new todo with the same id as the above one.
        Todo invalidTodo = new Todo("example todo again", 5, false);
        result = service.AddTodo(invalidTodo);
        
        //Newly added todo should be null, and should return 409
        Assert.Null(result.Data);
        Assert.Equal(409, result.StatusCode);

        //This error message should be returned when an invalid todo is received by the AddTodo service.
        Assert.Equal($"Todo with id of \'{invalidTodo.id}\' already exists!", result.Message);
    }

    [Fact]
    //Test to see if a adding a invalid todo with a duplicate name return null and 409
    //EXPECTED: Null
    //STATUS CODE: 409
    public void TestAddInvalidTodoDuplicateName(){
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
        Assert.Equal($"Todo with name of \'{invalidTodo.todoName}\' already exists!", result.Message);
    }

    [Fact]
    //Test to see if a deleting a valid todo works.
    //EXPECTED: ServiceResult (Not Null)
    //STATUS CODE: 200
    public void TestDeleteValidTodoById(){
        TodoService service = new TodoService(new List<Todo>{
            new Todo("todo 1", 1, false),
            new Todo("todo 2", 2, true),
        });
        ServiceResult<Todo> result = service.DeleteTodoById(1);

        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
    }
    
}