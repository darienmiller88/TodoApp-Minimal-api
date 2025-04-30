using api.v1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.MapGet("/", () => "Hello Wold!");
app.MapGet("/get-todos", (TodoService service) => service.GetTodos());
app.MapGet("/get-todo/{id}", (int id) => {
   
});

app.Run();