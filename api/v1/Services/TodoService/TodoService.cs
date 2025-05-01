using api.v1.Models;
namespace api.v1.Services;

//Interface for Todoservice to allow for depency injection for each route.
interface ITodoService{
    Todo[] GetTodos();
    Todo GetTodoById(int id);
}

//Todo service implemenation that completes business logic for database interactions.
class TodoService : ITodoService{
    private Todo[] todos = [
        new Todo("Buy vicky birthday gift", 0),
        new Todo("Go to birthday party", 1),
        new Todo("Complete Todo list before day ends", 2)
    ];

    // This method returns an array of Todo objects from the database eventually.
    public Todo[] GetTodos(){
        return todos;
    }

    public Todo GetTodoById(int id){
        Todo? todo = todos.FirstOrDefault(todo => todo.id == id);

        // if (todo == null) {
        //     return Results.NotFound($"Todo with id: {id} not found.");
        // }
    

        return todo == null ? new Todo("nothing found", 999) : todo;
    }

    // Function to add a todo to the array,  and eventually to the database.
     public ServiceResult<Todo> AddTodo(Todo newTodo){

        //First, check to see if there is a todo with a duplicate ID in the list of todos
        if (todos.FirstOrDefault(todo => todo.id == newTodo.id) == null){
            return new ServiceResult<Todo>($"Todo with id of {newTodo.id} already exists!", 409, new Todo());
        }

        //Second, check to see if there is a todo with a duplicate name in the list of todos.
        if (todos.FirstOrDefault(todo => todo.todoName == newTodo.todoName) == null){
            return new ServiceResult<Todo>($"Todo with name of {newTodo.todoName} already exists!", 409, new Todo());
        }

        //Afterwards, add the new todo to the list.
        todos.ToList().Add(newTodo);
       
        //Send the todo back with a successful code of 201.
        return new ServiceResult<Todo>($"", 201, newTodo);
    }
};