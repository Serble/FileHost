using FileHostingApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace FileHostingApi.Controllers; 

[ApiController]
[Route("files")]
public class FilesController : Controller {
    
    [HttpGet("{fileName}")]
    public async Task<IActionResult> Get(string fileName) {
        if (Program.StorageService == null) throw new Exception();
        Stream? file = await Program.StorageService.GetFile(fileName);
        if (file == null) {
            return NotFound("File not found");
        }
        // Convert file to a steam and return it
        return File(file, "application/octet-stream");
    }
    
}