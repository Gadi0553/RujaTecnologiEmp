using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace RujaTecnologi.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string ImageBaseUrl = "https://localhost:7191/api/Files/images/";
        public ProductosController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Productos
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Productos>>> GetProductos()
        {
          
            return _context.Productos;
        }

        // GET: api/Productos/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Productos>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // POST: api/Productos
        // POST: api/Productos
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Productos>> PostProducto(Productos request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica que la categoría exista
            var categoria = await _context.Categorias.FindAsync(request.CategoriaId);
            if (categoria == null)
            {
                return BadRequest("La categoría especificada no existe.");
            }

            var producto = new Productos
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                Stock = request.Stock,
                ImagenURL = request.ImagenURL,
                CategoriaId = request.CategoriaId
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.ProductoId }, producto);
        }


        // PUT: api/Productos/5
        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, [FromForm] Productos producto, IFormFile? imagen)
        {
            if (id != producto.ProductoId)
            {
                return BadRequest();
            }

            var existingProducto = await _context.Productos.FindAsync(id);
            if (existingProducto == null)
            {
                return NotFound();
            }

            if (imagen != null)
            {
                // Usar el FilesController para guardar la nueva imagen
                var filesController = new FilesController(_webHostEnvironment);
                var result = await filesController.Upload(imagen) as ObjectResult;

                if (result?.Value is object fileResult &&
                    fileResult.GetType().GetProperty("fileName")?.GetValue(fileResult) is string fileName)
                {
                    // Construir la URL de la imagen
                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    producto.ImagenURL = $"{baseUrl}/Uploads/{fileName}";
                }
            }
            else
            {
                // Mantener la URL de la imagen existente si no se proporciona una nueva
                producto.ImagenURL = existingProducto.ImagenURL;
            }

            _context.Entry(existingProducto).CurrentValues.SetValues(producto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}