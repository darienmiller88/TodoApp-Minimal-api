using api.v1.Models.Todo;

class TodoService{
    // This is a mock service. In a real application, this would be replaced with a database or other data source.
    // The data is hardcoded for demonstration purposes.
    // In a real application, you would use dependency injection to inject the data source.

    // This method returns an array of Todo objects.
    public static Todo[] getTodos(){
        return [
            new Todo("Buy vicky birthday gift", 0),
            new Todo("Go to birthday party", 1),
            new Todo("Complete Todo list before day ends", 2)
        ];
    }
};