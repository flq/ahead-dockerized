using Ahead.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ahead.Web.Pages;

[IgnoreAntiforgeryToken]
public class Graph(IAheadGraphDatabase database) : PageModel
{
    
    [BindProperty]
    public string? Name { get; set; }

    public IReadOnlyList<User> Users { get; set; } = [];
    
    public async Task OnGet()
    {
        Users = await database.RunJob(new ListUsers());
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
        Users = await database.RunJob(new ListUsers());
    }
}

public class AddUser(string name) : IGremlinJob
{
    public Task Run(IGraphContext graphContext) => 
        graphContext.Run($"""g.addV("{nameof(User)}").property("{nameof(User.Name)}", "{name}")""");
}

public class ListUsers : IGremlinJob<IReadOnlyList<User>>
{
    public async Task<IReadOnlyList<User>> Run(IGraphContext graphContext)
    {
        var result = 
            await graphContext.Run(g => 
                g.V().HasLabel(nameof(User)).ValueMap<string, object>());
        return result
            .Select(valueMap => new User((string)((List<object>)valueMap[nameof(User.Name)])[0]))
            .ToList();
    }
}

public record User(string Name);