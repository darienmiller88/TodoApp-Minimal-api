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

#### Interesting quirks of C#

* Methods that return two value
* WebApplicationBuilder, which allows for the creation of a mega router
* Dependency injection
* Handler method parameters
* Razor Pages
* get and set keywords
* XUnit testing
* Extension member
* Named parameters
* Automatic response body binding to models

#### Methods that return two values

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

#### WebApplicationBuilder

The coolest feature I used in ASP.NET so far was the `WebApplicationBuilder` class, which comes from the `using Microsoft.AspNetCore.Builder;` namespace. This is an incredibly powerful tool that allows you create RESTful APIs with methods like `.MapGet`, `.MapPost`, `.MapPut`, `.MapDelete`, `.MapPatch`, `.Use()`, `.MapGroup()`, etc. The methods allow very clean, very boilerplate-free code with creating APis as you can see below:

```c#
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddRazorPages();

var app = builder.Build();
```
The five major operations, as well as the `.Use()` methods are all defined on that `app` variable. It's very node in design obviously, but with all of the power of the ASP.NET framework backing it.

#### Dependency injection

Dependency Injection is technique in C# where one class instance is registered as part of another class instance, which will allow methods of that instance to have access to the methods of the first class instance. It is what makes the `builder.Services.AddScoped<ITodoService, TodoService>();` method of the WebApplicationClass so powerful. Rather than each route handler to the MapRequests have a response and request object, each handler can essentially have as many parameters as there are services registered with the builder instance! 

In the above example, I added both my TodoService interface and TodoService instance as a Dependency for the builder class, so now when I add a handler to the `.MapXXX()` methods, I can pass `ITodoService` to them as a parameter!

#### Razor Pages


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Feel free to leave suggestions as well, I'm always looking for ways to improve!

<p align="right">(<a href="#top">back to top</a>)</p>

## License
[MIT](https://choosealicense.com/licenses/mit/)