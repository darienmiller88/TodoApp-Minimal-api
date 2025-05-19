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

public class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly ITodoService _service;
    public List<Todo> todos;

    [BindProperty]
    //Forms from front end will bind to this object
    public Todo NewTodo { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger, ITodoService service) {
        _logger = logger;
        _service = service;
        todos = [];
    }

    public async Task OnGet() {
        todos = await _service.GetTodosAsync();
    }

    public async Task<IActionResult> OnPost() {
        if (!ModelState.IsValid) {
            return Page();
        }

        //Add the new todo to MongoDB...
        await _service.AddTodoAsync(NewTodo);

        //And afterwards, refresh the list of Todos by retrieving the updated list from the database.
        todos = await _service.GetTodosAsync();

        return RedirectToPage(); // Redirect back to the page.
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id) {
        Console.WriteLine("Delete called! id: " + id);

        return RedirectToPage(); // Redirect to GET
    }
}
