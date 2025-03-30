using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ahead.Web.Pages;

public class Index : PageModel
{
    public string? Message { get; set; }
    
    public void OnGet(string? message = null)
    {
        Message = message;
    }
}