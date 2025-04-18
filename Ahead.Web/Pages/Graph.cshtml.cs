using Ahead.Common;
using Ahead.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ahead.Web.Pages;

[IgnoreAntiforgeryToken]
public class Graph(IAheadGraphDatabase database) : PageModel
{
    
    
    public async Task OnGet(bool? loadFiles = false)
    {
    
    }

    public async Task OnPost(CancellationToken cancellationToken)
    {
        
    }
}

