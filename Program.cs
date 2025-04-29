using api.v1.Models.Todo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Todo []todos = {
    new Todo("Buy vicky birthday gift", 0),
    new Todo("Go to birthday party", 1),
    new Todo("Complete Todo list before day ends", 2)
};

app.MapGet("/", () => "Hello Wold!");
app.MapGet("/get-todos", () => todos);
app.MapGet("/get-todo/{id}", (int id) => {
    var todo = todos.FirstOrDefault(todo => todo.id == id);

    if (todo == null) {
        return Results.NotFound($"Todo with id: {id} not found.");
    }

    return Results.Ok(todo);
});

app.Run();