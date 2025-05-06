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
    }

    [Fact]
    //Test to see if all incompleted todos are returned. It SHOULD NOT be null.
    public void TestGetIncompletedTodos(){
        TodoService service = new TodoService();
        ServiceResult<List<Todo>> result = service.GetIncompletedTodos();

        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
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
    }
}