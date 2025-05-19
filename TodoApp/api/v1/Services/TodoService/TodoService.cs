using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.v1.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace api.v1.Services;

//stringerface for Todoservice to allow for depency injection for each route.
public interface ITodoService{
    Task<List<Todo>> GetTodosAsync();
    Task<ServiceResult<List<Todo>>> GetIncompletedTodosAsync();
    Task<ServiceResult<List<Todo>>> GetCompletedTodosAsync();
    Task<ServiceResult<Todo>> GetTodoByIdAsync(string Id);
    Task<ServiceResult<Todo>> AddTodoAsync(Todo newTodo);
    Task<ServiceResult<Todo>> DeleteTodoByIdAsync(string Id);
    Task<ServiceResult<UpdateResult>> UpdateTodoByIdAsync(string Id);
    Task<ServiceResult<UpdateResult>> UpdateTodoByNameAsync(string Id, Todo newTodo);
}

//Todo service implemenation that completes business logic for database stringeractions.
public class TodoService : ITodoService{
    private readonly IMongoCollection<Todo> todoCollection;
    public TodoService(){
        MongoClient mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGO_URI"));
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DATABASE"));
        todoCollection = mongoDatabase.GetCollection<Todo>("todos");
    }

    public TodoService(IMongoCollection<Todo> todoCollection){
        this.todoCollection = todoCollection;
    }

    //GET: This method returns an array of Todo objects from the database eventually.
    public async Task<List<Todo>> GetTodosAsync(){
       return await todoCollection.Find(_ => true).ToListAsync();
    }

    //GET: Get a todo that with an Id of 'Id'
    public async Task<ServiceResult<Todo>> GetTodoByIdAsync(string Id){
        Todo? todoResult = await todoCollection.Find(todo => todo.Id == Id).FirstOrDefaultAsync();

        //If there is no todo with an Id of 'Id' found in the list of todos, return 404 not found.
        if (todoResult == null) {
            return new ServiceResult<Todo>($"Todo with Id: {Id} not found.", 404, null);
        }

        //otherwise return a 200 and the todo that was found.
        return new ServiceResult<Todo>("Found todo!", 200, todoResult);
    }

    //GET: Retrieves all todos that are completed.
    public async Task<ServiceResult<List<Todo>>> GetCompletedTodosAsync(){
        List<Todo> completedTodos = await todoCollection.Find(todo => todo.isComplete).ToListAsync();

        return new ServiceResult<List<Todo>>("All completed todos", 200, completedTodos);
    }

    //GET: Retrieves all todos that are not completed.
    public async Task<ServiceResult<List<Todo>>> GetIncompletedTodosAsync(){
        List<Todo> incompletedTodos = await todoCollection.Find(todo => !todo.isComplete).ToListAsync();

        return new ServiceResult<List<Todo>>("All incompleted todos", 200, incompletedTodos);
    }

    //POST: Function to add a todo to the array,  and eventually to the database.
     public async Task<ServiceResult<Todo>> AddTodoAsync(Todo newTodo){
        
        //First, check to see if there is a todo with a duplicate name in the list of todos
        if(await todoCollection.Find(todo => todo.TodoName.ToLower() == newTodo.TodoName.ToLower()).FirstOrDefaultAsync() != null){
            return new ServiceResult<Todo>($"Todo with name {newTodo.TodoName} is taken!", 409, null);
        }

        //Afterwards, add the new todo to the list.
        await todoCollection.InsertOneAsync(newTodo);
       
        //Send the todo back with a successful code of 201.
        return new ServiceResult<Todo>("Successfully added todo!", 201, newTodo);
    }

    //DELETE: Method to delete a todo by its Id.
    public async Task<ServiceResult<Todo>> DeleteTodoByIdAsync(string Id){
        Todo? todoToDelete = await todoCollection.Find(todo => todo.Id == Id).FirstOrDefaultAsync();

        //Try to find the todo that is to be deleted, and return an error if it doesn't exist.
        if (todoToDelete == null) {
            return new ServiceResult<Todo>($"No todo with Id {Id} found!", 404, null);
        }

        //If found, remove it!
        await todoCollection.DeleteOneAsync(todo => todo == todoToDelete);

        return new ServiceResult<Todo>("Todo deleted!", 200, todoToDelete);
    }

    //PATCH: Method to update a Todo's complete status from done to undone, and vice versa.
    public async Task<ServiceResult<UpdateResult>> UpdateTodoByIdAsync(string Id){
        Todo? todoToUpdate = await todoCollection.Find(todo => todo.Id == Id).FirstOrDefaultAsync();

        //Try to find the todo that is to be updated, and return an error if it doesn't exist.
        if (todoToUpdate == null){
            return new ServiceResult<UpdateResult>($"No todo with Id {Id} found!", 404, null);
        }

        //establish the filter to find the id of the todo to update
        var filter = Builders<Todo>.Filter.Eq(todo => todo.Id, Id);

        //As well as the update to determine which fields get updated. 
        var update = Builders<Todo>.Update.Set(todo => todo.isComplete, !todoToUpdate.isComplete).Set(todo => todo.UpdatedAt, DateTime.Now.ToString());
        
        //Finally, send both the filter and update to the update.
        var updateResult = await todoCollection.UpdateOneAsync(filter, update);

        //Return the newly update todo.
        return new ServiceResult<UpdateResult>("Todo updated!", 200, updateResult);
    }

    //PATCH: Method to change a Todos name.
    public async Task<ServiceResult<UpdateResult>> UpdateTodoByNameAsync(string Id, Todo newTodo){
        Todo? todoToUpdate = await todoCollection.Find(todo => todo.Id == Id).FirstOrDefaultAsync();

        //Try to find the todo that is to be updated, and return an error if it doesn't exist.
        if (todoToUpdate == null) {
            return new ServiceResult<UpdateResult>($"No todo with Id {Id} found!", 404, null);
        }

        //establish the filter to find the id of the todo to update
        var filter = Builders<Todo>.Filter.Eq(todo => todo.Id, Id);
        
        //Create update filter to determine which fields get updated. 
        var update = Builders<Todo>.Update.Set(todo => todo.TodoName, newTodo.TodoName).Set(todo => todo.UpdatedAt, DateTime.Now.ToString());
        
        //Finally, send both the filter and update to the update.
        var updateResult = await todoCollection.UpdateOneAsync(filter, update);

        //Return the newly updated todo!
        return new ServiceResult<UpdateResult>("Todo updated!", 200, updateResult);
    } 
};