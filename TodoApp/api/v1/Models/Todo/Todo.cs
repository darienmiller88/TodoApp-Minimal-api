using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.v1.Models;

public class Todo{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("todo_name")]
    [StringLength(25, MinimumLength = 5)]
    public string TodoName { get; set; }

    [BsonElement("is_complete")]
    public bool isComplete { get; set; }

    public Todo(){
        TodoName = "";
        Id = "";
        isComplete = false;
    }
    
    public Todo(string TodoName, bool completeStatus){
        this.TodoName = TodoName.Trim();
        Id = "";
        isComplete = completeStatus;
    }

    public override bool Equals(object? obj){
        return obj is Todo other && TodoName == other.TodoName && Id == other.Id && isComplete == other.isComplete;
    }

    public override int GetHashCode(){
        return HashCode.Combine(TodoName, Id, isComplete);
    }

    public override string ToString(){
        return $"todo name: {TodoName}\n id: {Id}\n isComplete: {isComplete}";
    }
}