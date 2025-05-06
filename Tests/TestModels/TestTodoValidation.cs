using Xunit;
using api.v1.Models;
using MiniValidation;
namespace Tests;

public class TestTodoValidation{

    [Fact]
    public void TestValidTodoName(){
        Todo todo = new Todo("finish todo app", 1);
        bool isValid = MiniValidator.TryValidate(todo, out var errors);
        
        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Fact]
    //Test a todo with a name that is too short, less than the 5 characters minimum.
    public void TestInvalidTodoNameTooShort(){
        Todo todo = new Todo("fin", 1);
        bool isValid = MiniValidator.TryValidate(todo, out var errors);
        
        Assert.False(isValid);
    }

    [Fact]
    //Test a todo with a name that is too long, more than the 50 characters maximum.
    public void TestInvalidTodoNameTooLong(){
        Todo todo = new Todo("finish todo appfinish todo appfinish todo appfinish finish todo appfinish todo appfinish todo apptodo appfinish todo appfinish todo app", 1);
        bool isValid = MiniValidator.TryValidate(todo, out var errors);
        
        Assert.False(isValid);
    }
}