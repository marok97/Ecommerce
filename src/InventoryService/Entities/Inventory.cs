using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Entities
{
    [Table("inventory")]
    [Index(nameof(ProductId))]
    [Index(nameof(Sku), IsUnique = true)]
    public class Inventory
    {
        [Key] [Column("id")] public Guid Id { get; set; } = Guid.NewGuid();

        [Required] [Column("product_id")] public Guid ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("sku")]
        public string Sku { get; set; } = string.Empty;

        [Required]
        [Column("quantity_available")]
        public int QuantityAvailable { get; set; } = 0;

        [Required]
        [Column("quantity_reserved")]
        public int QuantityReserved { get; set; } = 0;

        [Required]
        [MaxLength(50)]
        [Column("warehouse_location")]
        public string WarehouseLocation { get; set; } = string.Empty;

        [Required] [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"<Inventory(id={Id}, sku={Sku}, available={QuantityAvailable}, reserved={QuantityReserved})>";
        }
    }
}