using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RujaTecnologi
{
    public class Productos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductoId { get; set; } // Clave primaria

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } // Nombre del producto

        public string Descripcion { get; set; } // Descripción del producto

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Precio { get; set; } // Precio del producto

        public string? ImagenURL { get; set; } // URL de la imagen

        [Required]
        public int Stock { get; set; } // Cantidad disponible en inventario

        // Clave foránea: Relación con Categorias
        [ForeignKey("Categoria")]
        public int CategoriaId { get; set; }

        public Categoria Categoria { get; set; } // Propiedad de navegación
    }

    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoriaId { get; set; } // Clave primaria

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } // Nombre de la categoría

        [StringLength(255)]
        public string Descripcion { get; set; } // Descripción de la categoría


        // Relación: Una categoría tiene muchos productos
        public ICollection<Productos> Productos { get; set; } = new List<Productos>();
    }
}
