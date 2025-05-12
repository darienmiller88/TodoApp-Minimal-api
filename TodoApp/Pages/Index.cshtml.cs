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
public class IndexModel : PageModel{
    private readonly ILogger<IndexModel> _logger;
    private readonly ITodoService _service;
    public List<Todo> todos; 

    [BindProperty]
    //Forms from front end will bind to this object
    public Todo NewTodo { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger, ITodoService service){
        _logger = logger;
        _service = service;
        todos = [];
    }

    public void OnGet(){
        todos = _service.GetTodos();
        
        foreach (Todo todo in todos){
            Console.WriteLine(todo);
        }
    }

    public IActionResult OnPost(){
        if (!ModelState.IsValid){
            return Page();
        }

        
        NewTodo.id = _service.GetTodos().Last().id + 1;
        var result = _service.AddTodo(NewTodo);

        Console.WriteLine(NewTodo);
        todos = _service.GetTodos();
        
        if (result.Data == null){
            Console.WriteLine(result.Message + " " + result.StatusCode);
        }else{
        }


        return RedirectToPage(); // Redirect to GET
    }

}
