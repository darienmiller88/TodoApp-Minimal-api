namespace api.v1.Services;
using api.v1.Models;

//Interface for Todoservice to allow for depency injection for each route.
interface ITodoService{
     Todo[] GetTodos();
}

//Todo service implemenation that completes business logic for database interactions.
class TodoService : ITodoService{

    // This method returns an array of Todo objects from the database eventually.
    public Todo[] GetTodos(){
        return [
            new Todo("Buy vicky birthday gift", 0),
            new Todo("Go to birthday party", 1),
            new Todo("Complete Todo list before day ends", 2)
        ];
    }

    public Todo GetTodoById(int id){
        return new Todo("blaj", 9);
    }

};