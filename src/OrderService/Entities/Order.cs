using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Entities;

[Table("orders")]
public class Order
{
    [Key] [Column("id")] public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    [Column("customer_email")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [Column("status")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    [Column("total_amount", TypeName = "numeric(10,2)")]
    public decimal TotalAmount { get; set; }

    [Required] [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required] [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
