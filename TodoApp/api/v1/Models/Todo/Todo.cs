using System;
using System.ComponentModel.DataAnnotations;

namespace api.v1.Models;

public class Todo{

    [StringLength(50, MinimumLength = 5)]
    public string todoName { get; set; }
    public int id { get; set; }
    public bool isComplete { get; set; }

    public Todo(){
        todoName = "";
        id = 0;
        isComplete = false;
    }
    
    public Todo(string todoName, int id, bool completeStatus){
        this.todoName = todoName.Trim();
        this.id = id;
        isComplete = completeStatus;
    }

    public override bool Equals(object? obj){
        return obj is Todo other && todoName == other.todoName && id == other.id && isComplete == other.isComplete;
    }

    public override int GetHashCode(){
        return HashCode.Combine(todoName, id, isComplete);
    }

    public override string ToString(){
        return $"todo name: {todoName}\n id: {id}\n isComplete: {isComplete}";
    }
}