using Xunit;
using api.v1.Models;
using MiniValidation;
namespace Tests;

public class TestTodoValidation{

    [Fact]
    public void TestValidTodoName(){
        Todo todo = new Todo("finish todo app", false);
        bool isValid = MiniValidator.TryValidate(todo, out var errors);
        
        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Fact]
    //Test a todo with a name that is too short, less than the 5 characters minimum.
    public void TestInvalidTodo_NameTooShort(){
        Todo todo = new Todo("fin", false);
        bool isValid = MiniValidator.TryValidate(todo, out var errors);
        
        Assert.False(isValid);
        Assert.NotEmpty(errors);
    }

    [Fact]
    //Test a todo with a name that is too long, more than the 50 characters maximum.
    public void TestInvalidTodo_NameTooLong(){
        Todo todo = new Todo("finish todo appfinish todo appfinish todo appfinish finish todo appfinish todo appfinish todo apptodo appfinish todo appfinish todo app", true);
        bool isValid = MiniValidator.TryValidate(todo, out var errors);
        
        Assert.False(isValid);
        Assert.NotEmpty(errors);
    }

    [Fact]
    //Test a todo with a name that is all whitespace.
    public void TestInvalidTodoNameAllWhiteSpace(){
        Todo todo = new Todo("     ", true);
        bool isValid = MiniValidator.TryValidate(todo, out var errors);
        
        Assert.False(isValid);
        Assert.NotEmpty(errors);
    }
}