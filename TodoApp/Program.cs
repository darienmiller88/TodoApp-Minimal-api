using api.v1.Services;
using api.v1.Middlewares;
using api.v1.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//'AddSingleton' ensures that the TodoService instance is shared for all routes, and persists as long as the server is
//running. NOTE: this is just temporary until I get database functionality up and running. Afterwards, I will switch
//back to AddScoped.
builder.Services.AddSingleton<ITodoService, TodoService>();

var app = builder.Build();

app.Use(Logger.LogRequestAsync);

app.MapGet("/", () => "visit prefeix /api/v1/todos for api content");
app.MapTodoRoutes();

app.Run();