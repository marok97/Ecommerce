using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Entities;

[Table("products")]
[Index(nameof(Sku), IsUnique = true)]
[Index(nameof(Category))]
public class Product
{
    [Key] [Column("id")] public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    [Column("sku")]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("price", TypeName = "numeric(10,2)")]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("category")]
    public string Category { get; set; } = string.Empty;

    [Required] [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required] [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
