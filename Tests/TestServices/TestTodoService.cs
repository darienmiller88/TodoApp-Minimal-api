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
    //Test to see if all incompleted todos are returned.
    public void TestGetIncompletedTodos(){
        TodoService service = new TodoService();
        ServiceResult<List<Todo>> result = service.GetIncompletedTodos();

        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }
}