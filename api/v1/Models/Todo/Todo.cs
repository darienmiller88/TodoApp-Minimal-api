namespace api.v1.Models.Todo;

class Todo{
    public string TodoName { get; set; }
    public int id { get; set; }

    public Todo(string todoName, int id){
        TodoName = todoName;
        this.id = id;
    }
}