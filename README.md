# Todo app using C# Minimal API

I'm introducing a new language into my tech stack: C#! Super happy with how the language functions: minimal api is EXTREMELY clean, but i'm a bit overwhelmed lol.

### Built With:
* [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
* [MongoDB-Atlas](https://www.mongodb.com/cloud/atlas)
* [Azure](https://azure.microsoft.com/en-us/pricing/purchase-options/azure-account?icid=portal)
* [Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-9.0&tabs=visual-studio)

### Requirements:

* Clone repo using `git clone https://github.com/darienmiller88/TodoApp-Minimal-api`
* Migrate the necessary information to your local `.env` as described in the `.env_sample` file
* Run `dotnet run`, or `dotnet watch run` to enable a server restart on change.
* `cd client` to access the Vue package, and run `npm run dev` to start vite server for Vue, which should be on port `5173`.
* Run `dotnet test` to run tests.

### Website link:

Visit the webapp at this funky ass link -> https://razortodoapp-bvcce2h9e5hyg5d5.canadacentral-01.azurewebsites.net/

### What I learned:

When delving into this mini project, my goal was to build foundational knowledge using ASP.NET to build effective
and scable web applications in a brand language, as well as a new environment. The infrastructure, the tooling, the 
library, and the language features are all really cool! Here's what stood out:

### Interesting quirks of C#

* Methods that return two value
* WebApplicationBuilder, which allows for the creation of a mega router
* Dependency injection
* Handler method parameters
* Razor Pages
* get and set keywords
* XUnit testing
* Extension methods
* Named parameters
* Automatic response body binding to models

### Methods that return two values

In c#, in a similar vein to go, can return more than one return value, though with a very different syntax as shown below:

```c#
if (context.Request.RouteValues.TryGetValue("id", out var id)) {
    string? idString = id?.ToString();

    if (!ObjectId.TryParse(idString, out _)) {
        await context.Response.WriteAsync($"{idString} is not a valid 24 hex string");
        return;
    }
}
```

As you can see with the second argument to `TryGetValue()`, `out var id` returns the value for the route parameter the if statement is parsing. `out` declares that the method is returning a variable, and var id is variable declaration. Of course, if you don't care about the value being returned, you can do this `out _` to tell the compiler to ignore the return value.

### WebApplicationBuilder

The coolest feature I used in ASP.NET so far was the `WebApplicationBuilder` class, which comes from the `using Microsoft.AspNetCore.Builder;` namespace. This is an incredibly powerful tool that allows you create RESTful APIs with methods like `.MapGet`, `.MapPost`, `.MapPut`, `.MapDelete`, `.MapPatch`, `.Use()`, `.MapGroup()`, etc. The methods allow very clean, very boilerplate-free code with creating APis as you can see below:

```c#
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddRazorPages();

var app = builder.Build();
```
The five major operations, as well as the `.Use()` methods are all defined on that `app` variable. It's very node in design obviously, but with all of the power of the ASP.NET framework backing it.

### Dependency injection

Dependency Injection is technique in C# where one class instance is registered as part of another class instance, which will allow methods of that instance to have access to the methods of the first class instance. It is what makes the `builder.Services.AddScoped<ITodoService, TodoService>();` method of the WebApplicationClass so powerful. Rather than each route handler to the MapRequests have a response and request object, each handler can essentially have as many parameters as there are services registered with the builder instance! 

In the above example, I added both my TodoService interface and TodoService instance as a Dependency for the builder class, so now when I add a handler to the `.MapXXX()` methods, I can pass `ITodoService` to them as a parameter!

### Razor Pages

Razor pages in c# are a neat feature that fully leverages Dependency injection to allow users to inject c# code into HTML pages, specifically pages with a `.cshtml` extension. These pages also have an additional `.cshtml.cs` file attached to them with an `IndexModel` class with methods and members that interact with the html code in the cshtml file. You can now write c# directly in the html code! You include anti CSRF tokens, reference your Models folder to retrieve model data, and much more! Here is a code snippet:

```c#
@if(todo.isComplete){
    <div class="todo-name strike-through">@todo.TodoName</div>
    <div class="delete-update-wrapper">
        <form asp-page-handler="Update" method="post">
            <button class="undo base-button-stlying">Undo</button>
            <input type="hidden" name="id" value="@todo.Id"/>
        </form>
        <form asp-page-handler="Delete" method="post">
            <button class="delete-todo base-button-stlying">Delete</button>

            @* This is how forms are able to send parameters to methods to the Handlers in the .cshtml.cs files *@
            <input type="hidden" name="id" value="@todo.Id"/>
        </form>
    </div>
}else{
    <div class="todo-name">@todo.TodoName</div>
    <div class="delete-update-wrapper">
        <form asp-page-handler="Update" method="post">
            <button class="complete base-button-stlying">Complete</button>
            <input type="hidden" name="id" value="@todo.Id"/>
        </form>
        <form asp-page-handler="Delete" method="post">
            <button class="delete-todo base-button-stlying">Delete</button>

            @* This is how forms are able to send parameters to methods to the Handlers in the .cshtml.cs files *@
            <input type="hidden" name="id" value="@todo.Id"/>
        </form>
    </div>
}
```

This tag right here:

```html
<form asp-page-handler="Update" method="post">
    <button class="complete base-button-stlying">Complete</button>
    <input type="hidden" name="id" value="@todo.Id"/>
</form>
```

has an attribute called `asp-page-handler` which allows this form to send a Post request to a method in the supporting `.cshtml.cs` called `OnPostUpdateAsync()` in the IndexModel class as shown below:

```c#
public async Task<IActionResult> OnPostUpdateAsync(string id) {

    //Update the todo be flipping its complete status.
    await _service.UpdateTodoByIdAsync(id);

    //Refresh the list of todos.
    todos = await _service.GetTodosAsync();

    //Finally, Refresh the page to show the new list.
    return RedirectToPage();
}
```
### get and set keywords

`get` and `set` in C# are shorthand ways to attach a getter and setter to a member in C#. Rather than have fully typed out setters and getters like in Java, you can just do the following:

```C#
public bool isComplete { get; set; }
```

For private members, the flow with look more like the Java workflow.

### XUnit testing

For testing purposes, I had a lot of options, but chose XUnit due to its implicty and familiar syntax after working with unit testing in Java. From what I've seen, it seems to be a solid, but standard testing library, contain the `Assert` class with the expected testing methods like `.Equal()`, `.NotNull()`, `.Null()`, etc. Here is a code snippet!

```c#
[Fact]
public async Task TestAddValidTodo(){
    var mockCollection = MockTodoService.GetMockTodoService(new List<Todo>{});
    TodoService service = new TodoService(mockCollection.Object);
    ServiceResult<Todo> result = await service.AddTodoAsync(new Todo("example todo", false));

    Assert.NotNull(result.Data);
    Assert.Equal(201, result.StatusCode);

    List<Todo> todos = await service.GetTodosAsync();

    //Assert that only one todo is in the fake database.
    Assert.Single(todos);
}
```

### Extension methods

This is easily one of the coolest features I've ever seen in a programming language! In c#, if make a class static, and any of its methods also static, you can attach that method to another class. That is, when the setup is complete, the `string` class for example will have an extra callable method I defined that is now technically apart of the class, and is treated as such! Here is an example of my usage:

```c#
public static class TodoRoutes{

    //Extension method to all it to be callable by the "WebApplication" class, which is what the "app" object is 
    //and instance of.
    public static void MapTodoRoutes(this IEndpointRouteBuilder app){
        RouteGroupBuilder todoRoutes = app.MapGroup("/api/v1/todos");

        //GET routes.
        todoRoutes.MapGet("/get-todos", GetTodosHandler);
        todoRoutes.MapGet("/get-completed-todos", GetCompletedTodosHandler);
        todoRoutes.MapGet("/get-incompleted-todos", GetIncompletedTodosHandler);
        todoRoutes.MapGet("/get-todo/{id}", GetTodoByIdHandler);
    }
}
```

As you can see, by using `this ClassName variableName`, you've officially made this method callable to the ClassName you typed in! In this example, `RouteGroupBuilder` is the class where the `app` variable that allows routing, so now I'm able to call `app.MapTodoRoutes()` and register my routes.

### Named parameters
When called a method in C#, you can just passing the arguments in the order as defined by the methods, OR if you prefer, in any order as long as you the name of method parameter in addition to the actual data as such:

```c#
return Results.Json(todos, statusCode: 200);
```

`Results.Json()` will take in the data to marshall into JSON, as well as a number of optional arguments with default values. Since I only needed to pass the status code 200, I only cared about the `statusCode` argument, which ended up being the last paramter in the parameter list for that method. So to get around that, all I have to do is type the name of the parameter, and boom!

### Automatic response body binding to models
Finally, unlike Go, my other backend language, c# has automatic model binding! So when giving a handler to one of the `.MapXXX()` methods, you can add the model you want the request body to bind to. Go requires this to be done manually with 
`Json.NewDecoder(req.Body).UnMarshall(&model)`, and in Node, the request comes in as a unstructured JavaScript object. Useful feature! Here is an example of me using it:

```c#
internal static async Task<IResult> AddTodoHandler(ITodoService service, [FromBody] Todo? todo) {

}
```

This handler will accept the TodoService I registered with the main `app` object in Program.cs, as well as the `Todo` model I would like the request body to bind to.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Feel free to leave suggestions as well, I'm always looking for ways to improve!

<p align="right">(<a href="#top">back to top</a>)</p>

## License
[MIT](https://choosealicense.com/licenses/mit/)