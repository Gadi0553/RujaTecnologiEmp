using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RujaTecnologi.Controllers
{
    [Route("api/Productos")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _appDbContext;

        public ProductosController(IWebHostEnvironment webHostEnvironment, AppDbContext appDbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _appDbContext = appDbContext;
        }

        // GET: api/<ProductosController>
        [HttpGet]
        public IEnumerable<Productos> Get()
        {
            return _appDbContext.Productos;
        }

        // GET api/<ProductosController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductosController>
        // POST api/<ProductosController>
        [AllowAnonymous]
        [HttpPost(Name = nameof(CreateProducto))]
        public IActionResult CreateProducto(Productos request)
        {
            var producto = new Productos
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                Stock = request.Stock,
                CategoriaId = request.CategoriaId
            };

            if (!string.IsNullOrEmpty(request.ImagenURL))
            {
                var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads", request.ImagenURL);
                if (System.IO.File.Exists(filePath))
                {
                    producto.ImagenURL = request.ImagenURL;
                }
            }

            _appDbContext.Productos.Add(producto);
            _appDbContext.SaveChanges();

            return Ok(producto);
        }


        // PUT api/<ProductosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
