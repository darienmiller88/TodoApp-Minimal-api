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
    private readonly ILogger<Index> _logger;
    private readonly ITodoService _service;
    public List<Todo> todos; 

    public IndexModel(ILogger<Index> logger, ITodoService service){
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
}
