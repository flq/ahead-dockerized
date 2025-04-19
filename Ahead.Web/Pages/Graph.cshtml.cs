using Ahead.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ahead.Web.Pages;

[IgnoreAntiforgeryToken]
public class Graph(IAheadGraphDatabase database) : PageModel
{
    
    [BindProperty]
    public string? Name { get; set; }
    
    public Task OnGet()
    {
        return Task.CompletedTask;
    }

    public async Task OnPost(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(Name))
        {
            TempData["Message"] = "No name, no user";
            return;
        }
        await database.RunJob(new AddUser(Name));
        TempData["Message"] = "User created";
    }
}

public class AddUser(string name) : IGremlinJob
{
    public async Task Run(IGraphContext graphContext)
    {
        
        await graphContext.Run($"""g.addV("User").property("Name", "{name}")""");
    }
}

