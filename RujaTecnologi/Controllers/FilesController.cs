using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RujaTecnologi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        const string UPLOADS_DIRECTORY_NAME = "Uploads";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
            var uploadPath = Path.Combine(_webHostEnvironment.ContentRootPath, UPLOADS_DIRECTORY_NAME, fileName);
            using var fileStream = System.IO.File.OpenWrite(uploadPath);
            await file.CopyToAsync(fileStream);

            return Ok(new
            {
                fileName
            });
        }
    }

}
