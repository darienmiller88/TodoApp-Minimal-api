using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.v1.Models;
using MongoDB.Driver;

namespace api.v1.Services;

//stringerface for Todoservice to allow for depency injection for each route.
public interface ITodoService{
    Task<List<Todo>> GetTodosAsync();
    Task<ServiceResult<List<Todo>>> GetIncompletedTodosAsync();
    Task<ServiceResult<List<Todo>>> GetCompletedTodos();
    Task<ServiceResult<Todo>> GetTodoById(string Id);
    Task<ServiceResult<Todo>> AddTodo(Todo newTodo);
    Task<ServiceResult<Todo>> DeleteTodoById(string Id);
    Task<ServiceResult<Todo>> UpdateTodoById(string Id);
    Task<ServiceResult<Todo>> UpdateTodoByName(string Id, Todo newTodo);
}

//Todo service implemenation that completes business logic for database stringeractions.
public class TodoService : ITodoService{
    private List<Todo> todos;
    private readonly IMongoCollection<Todo> todoCollection;
    public TodoService(){
        todos = [
            new Todo("Buy vicky birthday gift", false),
            new Todo("Go to birthday party", true),
            new Todo("Complete Todo list before day ends", false),
            new Todo("Go to sleep", true)
        ];

        MongoClient mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGO_URI"));
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DATABASE"));
        todoCollection = mongoDatabase.GetCollection<Todo>("todos");
    }

    // public TodoService(List<Todo> todos){
    //     this.todos = todos;

    //     MongoClient mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGO_URI"));
    //     IMongoDatabase mongoDatabase = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DATABASE"));
    //     todoCollection = mongoDatabase.GetCollection<Todo>("todos");
    // }

    //GET: This method returns an array of Todo objects from the database eventually.
    public async Task<List<Todo>> GetTodosAsync(){
       return await todoCollection.Find(_ => true).ToListAsync();
    }

    //GET: Get a todo that with an Id of 'Id'
    public ServiceResult<Todo> GetTodoById(string Id){
        Todo? todo = todos.FirstOrDefault(todo => todo.Id == Id);

        //If there is no todo with an Id of 'Id' found in the list of todos, return 404 not found.
        if (todo == null) {
            return new ServiceResult<Todo>($"Todo with Id: {Id} not found.", 404, null);
        }

        //otherwise return a 200 and the todo that was found.
        return new ServiceResult<Todo>("Found todo!", 200, todo);
    }

    //GET: Retrieves all todos that are completed.
    public ServiceResult<List<Todo>> GetCompletedTodos(){
        List<Todo> completedTodos = todos.FindAll(todo => todo.isComplete);

        //If there are no completed todos, return a 404 to the client telling them such.
        if (completedTodos.Count == 0){
            return new ServiceResult<List<Todo>>("No todos are completed... yet.", 404, null);
        }

        return new ServiceResult<List<Todo>>("All completed todos", 200, completedTodos);
    }

    //GET: Retrieves all todos that are not completed.
    public ServiceResult<List<Todo>> GetIncompletedTodos(){
        List<Todo> incompletedTodos = todos.FindAll(todo => !todo.isComplete);

        //If there are no INCOMPLETE todos, return a 404 to the client telling them such.
        if (incompletedTodos.Count == 0){
            return new ServiceResult<List<Todo>>("All todos are complete.", 404, null);
        }

        return new ServiceResult<List<Todo>>("All incompleted todos", 200, incompletedTodos);
    }

    //POST: Function to add a todo to the array,  and eventually to the database.
     public ServiceResult<Todo> AddTodo(Todo newTodo){

        //First, check to see if there is a todo with a duplicate Id in the list of todos
        if (todos.Any(todo => todo.Id == newTodo.Id)){
            return new ServiceResult<Todo>($"Todo with Id of \'{newTodo.Id}\' already exists!", 409, null);
        }

        //Second, check to see if there is a todo with a duplicate name in the list of todos.
        if (todos.Any(todo => todo.TodoName.Trim().ToLower() == newTodo.TodoName.Trim().ToLower())){
            return new ServiceResult<Todo>($"Todo with name of \'{newTodo.TodoName}\' already exists!", 409, null);
        }

        //Afterwards, add the new todo to the list.
        todos.Add(newTodo);
       
        //Send the todo back with a successful code of 201.
        return new ServiceResult<Todo>("Successfully added todo!", 201, newTodo);
    }

    //DELETE: Method to delete a todo by its Id.
    public ServiceResult<Todo> DeleteTodoById(string Id){
        Todo? todoToDelete = todos.FirstOrDefault(todo => todo.Id == Id);

        //Try to find the todo that is to be deleted, and return an error if it doesn't exist.
        if (todoToDelete == null) {
            return new ServiceResult<Todo>($"No todo with Id {Id} found!", 404, null);
        }

        //If found, remove it!
        todos.Remove(todoToDelete);

        return new ServiceResult<Todo>("Todo deleted!", 200, todoToDelete);
    }

    //PATCH: Method to update a Todo's complete status from done to undone, and vice versa.
    public ServiceResult<Todo> UpdateTodoById(string Id){
        int todoIndex = todos.FindIndex(todo => todo.Id == Id);
        
        //Try to find the todo that is to be updated, and return an error if it doesn't exist.
        if (todoIndex == -1) {
            return new ServiceResult<Todo>($"No todo with Id {Id} found!", 404, null);
        }

        //Retrieve todo to update from the list of todos.
        Todo todoToUpdate = todos.ElementAt(todoIndex);

        //Change the complete status to false or true depending on if it is complete or not.
        todos.ElementAt(todoIndex).isComplete = !todoToUpdate.isComplete;

        //Return the newly update todo.
        return new ServiceResult<Todo>("Todo updated!", 200, todoToUpdate);
    }

    //PATCH: Method to change a Todos name.
    public ServiceResult<Todo> UpdateTodoByName(string Id, Todo newTodo){
        int todoIndex = todos.FindIndex(todo => todo.Id == Id);
        
        //Try to find the todo that is to be updated, and return an error if it doesn't exist.
        if (todoIndex == -1) {
            return new ServiceResult<Todo>($"No todo with Id {Id} found!", 404, null);
        }

        //Change the current name to the new one!
        todos.ElementAt(todoIndex).TodoName = newTodo.TodoName;

        //Return the newly updated todo!
        return new ServiceResult<Todo>("Todo name updated!", 200, todos.ElementAt(todoIndex));
    }
};