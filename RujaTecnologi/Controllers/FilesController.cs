using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RujaTecnologi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        const string UPLOADS_DIRECTORY_NAME = "Uploads";
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public FilesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        [HttpPost]
        [AllowAnonymous]
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

        [HttpGet("{fileName}")]
        [AllowAnonymous]
        public IActionResult GetFile(string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, UPLOADS_DIRECTORY_NAME, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"El archivo {fileName} no existe.");
            }

            // Intenta obtener el tipo de contenido del archivo
            if (!_contentTypeProvider.TryGetContentType(fileName, out string? contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileName);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllFiles()
        {
            var uploadsPath = Path.Combine(_webHostEnvironment.ContentRootPath, UPLOADS_DIRECTORY_NAME);

            if (!Directory.Exists(uploadsPath))
            {
                return Ok(new { files = new List<string>() });
            }

            var files = Directory.GetFiles(uploadsPath)
                               .Select(filePath => Path.GetFileName(filePath))
                               .ToList();

            return Ok(new { files });
        }
    }
}