using System;
using System.ComponentModel.DataAnnotations;

namespace api.v1.Models;

public class Todo{

    [StringLength(25, MinimumLength = 5)]
    public string TodoName { get; set; }
    public int id { get; set; }
    public bool isComplete { get; set; }

    public Todo(){
        TodoName = "";
        id = 0;
        isComplete = false;
    }
    
    public Todo(string TodoName, int id, bool completeStatus){
        this.TodoName = TodoName.Trim();
        this.id = id;
        isComplete = completeStatus;
    }

    public override bool Equals(object? obj){
        return obj is Todo other && TodoName == other.TodoName && id == other.id && isComplete == other.isComplete;
    }

    public override int GetHashCode(){
        return HashCode.Combine(TodoName, id, isComplete);
    }

    public override string ToString(){
        return $"todo name: {TodoName}\n id: {id}\n isComplete: {isComplete}";
    }
}