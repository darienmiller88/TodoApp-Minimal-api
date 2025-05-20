using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using api.v1.Services;
using api.v1.Models;

namespace TodoApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ITodoService _service;
    public List<Todo> todos;

    [BindProperty]
    //Forms from front end will bind to this object
    public Todo NewTodo { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger, ITodoService service)
    {
        _logger = logger;
        _service = service;
        todos = [];
    }

    //This method is called every time the "/" route is hit.
    public async Task OnGet()
    {
        todos = await _service.GetTodosAsync();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        //Add the new todo to MongoDB...
        await _service.AddTodoAsync(NewTodo);

        //And afterwards, refresh the list of Todos by retrieving the updated list from the database.
        todos = await _service.GetTodosAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        //Get the id from the todo that's about to be deleted. The good thing is that the Id is guranteed to be correct
        //because the Todos are rendered with their mongo ids as their tag id!
        await _service.DeleteTodoByIdAsync(id);

        //Afterwards, refresh the current list of todos to reflect the current list.
        todos = await _service.GetTodosAsync();

        //Refresh the page to show the new list.
        return RedirectToPage();
    }

    //UPDATE: Change the complete status of the 
    public async Task<IActionResult> OnPostUpdateAsync(string id) {

        //Update the todo be flipping its complete status.
        await _service.UpdateTodoByIdAsync(id);

        //Refresh the list of todos.
        todos = await _service.GetTodosAsync();

        //Finally, Refresh the page to show the new list.
        return RedirectToPage();
    }
}
