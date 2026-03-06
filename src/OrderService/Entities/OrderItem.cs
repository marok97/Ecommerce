using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Entities;

[Table("order_items")]
public class OrderItem
{
    [Key] [Column("id")] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] [Column("order_id")] public Guid OrderId { get; set; }

    [Required] [Column("product_id")] public Guid ProductId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("sku")]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("product_name")]
    public string ProductName { get; set; } = string.Empty;

    [Required] [Column("quantity")] public int Quantity { get; set; }

    [Required]
    [Column("unit_price", TypeName = "numeric(10,2)")]
    public decimal UnitPrice { get; set; }

    public Order Order { get; set; } = null!;
}
