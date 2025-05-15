using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.v1.Models;

public class Todo{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("created_at")]
    public string CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public string UpdatedAt { get; set; }
    
    [BsonElement("todo_name")]
    [StringLength(25, MinimumLength = 5)]
    public string TodoName { get; set; }

    [BsonElement("is_complete")]
    public bool isComplete { get; set; }

    public Todo(){
        TodoName = "";
        Id = "";
        isComplete = false;
        CreatedAt = DateTime.Now.ToString();
        UpdatedAt = CreatedAt;
    }
    
    public Todo(string TodoName, bool completeStatus){
        this.TodoName = TodoName.Trim();
        Id = "";
        isComplete = completeStatus;
        CreatedAt = DateTime.Now.ToString();
        UpdatedAt = CreatedAt;
    }

    //For testing purposes only. Id will be overwritten when being added to database by mongodb.
    public void AssignRandomMongoObjectId(){
        Id = ObjectId.GenerateNewId().ToString();
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