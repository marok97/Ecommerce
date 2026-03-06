using InventoryService.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Database;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Inventory> Inventories => Set<Inventory>();


    //Override on changes to update timestamp on update
    public override int SaveChanges()
    {
        UpdateTimestamp();
        return base.SaveChanges();
    }

    private void UpdateTimestamp()
    {
        var entries = ChangeTracker.Entries<Inventory>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}