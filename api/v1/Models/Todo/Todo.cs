using System.ComponentModel.DataAnnotations;

namespace api.v1.Models;

public class Todo{

    [StringLength(10, MinimumLength = 5, ErrorMessage = "Todo name must be between 5 and 100 characters.")]
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