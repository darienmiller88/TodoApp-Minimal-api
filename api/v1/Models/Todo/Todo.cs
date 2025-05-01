namespace api.v1.Models;

public class Todo{
    public string todoName { get; set; }
    public int id { get; set; }
    public Boolean isComplete { get; set; }
    public Todo(string todoName, int id){
        this.todoName = todoName;
        this.id = id;
        isComplete = false;
    }
}