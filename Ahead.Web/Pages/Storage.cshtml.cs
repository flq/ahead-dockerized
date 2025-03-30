using Ahead.Common;
using Ahead.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ahead.Web.Pages;

[IgnoreAntiforgeryToken]
public class Storage(IBlobStorage storage) : PageModel
{
    [BindProperty]
    public IFormFile? Upload { get; set; }
    
    public IReadOnlyCollection<FileLink>? FileLinks { get; set; }
    
    public async Task OnGet(bool? loadFiles = false)
    {
        if (loadFiles.GetValueOrDefault())
        {
            FileLinks = await storage.ListLinks(Constants.Storage.StaticBucket).ToListAsync();
        }
    }

    public async Task OnPost(CancellationToken cancellationToken)
    {
        if (Upload == null)
        {
            TempData["Message"] = "Nothing was uploaded";
            return;
        }
        var objectName = await storage.UploadFile(Constants.Storage.StaticBucket, Upload, cancellationToken);
        TempData["Message"] = "Uploaded and stored with id " + objectName;
        
    }
}

