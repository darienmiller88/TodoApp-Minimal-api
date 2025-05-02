namespace api.v1.Models;

public class Todo{
    public string todoName { get; set; }
    public int id { get; set; }
    public bool isComplete { get; set; }

    public Todo(){
        todoName = "";
        id = 0;
        isComplete = false;
    }
    public Todo(string todoName, int id){
        this.todoName = todoName;
        this.id = id;
        isComplete = false;
    }

    public override string ToString(){
        return $"todo name: {todoName}\n id: {id}\n isComplete: {isComplete}";
    }
}