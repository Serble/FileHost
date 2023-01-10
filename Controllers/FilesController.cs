using Microsoft.AspNetCore.Mvc;

namespace FileHostingApi.Controllers; 

[ApiController]
[Route("files")]
public class FilesController : Controller {
    
    [HttpGet("{fileId}")]
    public async Task<IActionResult> Get(string fileId, [FromQuery] string? name = null) {
        if (Program.StorageService == null) throw new Exception();
        name ??= fileId;
        Stream? file = await Program.StorageService.GetFile(fileId);
        if (file == null) {
            return NotFound();
        }
        Response.Headers.ContentDisposition = $"attachment; filename=\"{name}\"";
        return File(file, "application/octet-stream");
    }
    
}