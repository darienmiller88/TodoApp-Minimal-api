using api.v1.Models;
namespace api.v1.Services;

//Interface for Todoservice to allow for depency injection for each route.
public interface ITodoService{
    List<Todo> GetTodos();
    ServiceResult<List<Todo>> GetIncompletedTodos();
    ServiceResult<List<Todo>> GetCompletedTodos();
    ServiceResult<Todo> GetTodoById(int id);
    ServiceResult<Todo> AddTodo(Todo newTodo);
    ServiceResult<Todo> DeleteTodoById(int id);
    ServiceResult<Todo> UpdateTodoById(int id);
}

//Todo service implemenation that completes business logic for database interactions.
public class TodoService : ITodoService{
    private List<Todo> todos = [
        new Todo("Buy vicky birthday gift", 0),
        new Todo("Go to birthday party", 1),
        new Todo("Complete Todo list before day ends", 2)
    ];

    //GET: This method returns an array of Todo objects from the database eventually.
    public List<Todo> GetTodos(){
        return todos;
    }

    //GET: Get a todo that with an id of 'id'
    public ServiceResult<Todo> GetTodoById(int id){
        Todo? todo = todos.FirstOrDefault(todo => todo.id == id);

        //If there is no todo with an id of 'id' found in the list of todos, return 404 not found.
        if (todo == null) {
            return new ServiceResult<Todo>($"Todo with id: {id} not found.", 404, null);
        }

        //otherwise return a 200 and the todo that was found.
        return new ServiceResult<Todo>("", 200, todo);
    }

    //GET: Retrieves all todos that are completed.
    public ServiceResult<List<Todo>> GetCompletedTodos(){
        List<Todo> completedTodos = todos.FindAll(todo => todo.isComplete);

        //If there are no completed todos, return a 404 to the client telling them such.
        if (completedTodos.Count == 0){
            return new ServiceResult<List<Todo>>("No todos are completed... yet.", 404, null);
        }

        return new ServiceResult<List<Todo>>("", 200, completedTodos);
    }

    //GET: Retrieves all todos that are not completed.
    public ServiceResult<List<Todo>> GetIncompletedTodos(){
        List<Todo> incompletedTodos = todos.FindAll(todo => !todo.isComplete);

        //If there are no INCOMPLETE todos, return a 404 to the client telling them such.
        if (incompletedTodos.Count == 0){
            return new ServiceResult<List<Todo>>("All todos are complete.", 404, null);
        }

        return new ServiceResult<List<Todo>>("", 200, incompletedTodos);
    }

    //POST: Function to add a todo to the array,  and eventually to the database.
     public ServiceResult<Todo> AddTodo(Todo newTodo){

        //First, check to see if there is a todo with a duplicate ID in the list of todos
        if (todos.Any(todo => todo.id == newTodo.id)){
            return new ServiceResult<Todo>($"Todo with id of \'{newTodo.id}\' already exists!", 409, null);
        }

        //Second, check to see if there is a todo with a duplicate name in the list of todos.
        if (todos.Any(todo => todo.todoName == newTodo.todoName)){
            return new ServiceResult<Todo>($"Todo with name of \'{newTodo.todoName}\' already exists!", 409, null);
        }

        //Afterwards, add the new todo to the list.
        todos.Add(newTodo);
       
        //Send the todo back with a successful code of 201.
        return new ServiceResult<Todo>("Successfully added todo!", 201, newTodo);
    }

    public ServiceResult<Todo> DeleteTodoById(int id){
        Todo? todoToDelete = todos.FirstOrDefault(todo => todo.id == id);

        //Try to find the todo that is to be deleted, and return an error if it doesn't exist.
        if (todoToDelete == null) {
            return new ServiceResult<Todo>($"No todo with id {id}", 404, null);
        }

        //If found, remove it!
        todos.Remove(todoToDelete);

        return new ServiceResult<Todo>("Todo deleted!", 200, todoToDelete);
    }
    public ServiceResult<Todo> UpdateTodoById(int id){
        int todoIndex = todos.FindIndex(todo => todo.id == id);
        
        //Try to find the todo that is to be updated, and return an error if it doesn't exist.
        if (todoIndex == -1) {
            return new ServiceResult<Todo>($"No todo with id {id} found!", 404, null);
        }

        Todo todoToUpdate = todos.ElementAt(todoIndex);

        //Change the complete status to false or true depending on if it is complete or not.
        todos.ElementAt(todoIndex).isComplete = !todoToUpdate.isComplete;

        //Return the newly update todo.
        return new ServiceResult<Todo>("Todo updated!", 200, todoToUpdate);
    }
};