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
}