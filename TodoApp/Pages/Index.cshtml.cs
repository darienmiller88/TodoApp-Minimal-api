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

    public async Task OnGet(){
        todos = await _service.GetTodosAsync();
    }

    public async Task<IActionResult> OnPost(){
        if (!ModelState.IsValid){
            return Page();
        }

        
        // var result = _service.AddTodo(NewTodo);

        Console.WriteLine(NewTodo);
        todos = await _service.GetTodosAsync();
        
        // if (result.Data == null){
        //     Console.WriteLine(result.Message + " " + result.StatusCode);
        // }else{
        // }


        return RedirectToPage(); // Redirect to GET
    }

}
