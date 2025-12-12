using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportMania.Models;
using SportMania.Models.Interface;

namespace SportMania.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IHasAuditTimestamps>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.DeletedAt = null;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    // Prevent changing CreatedAt
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    break;

                case EntityState.Deleted:
                    // Soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                    // Prevent changing CreatedAt
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set default schema for all tables
        modelBuilder.HasDefaultSchema("SportMania");

        // Global query filters (soft delete)
        modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Plan>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Transaction>().HasQueryFilter(t => !t.IsDeleted);

        // Relationships
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Customer)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Plan)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        // Owned types
        modelBuilder.Entity<Transaction>().OwnsOne(t => t.Key, b =>
        {
            b.WithOwner().HasForeignKey("TransactionId");
            b.Property(k => k.Code).HasMaxLength(100);
            b.ToTable("TransactionKeys");
        });

        modelBuilder.Entity<Plan>().OwnsMany(p => p.Details, b =>
        {
            b.WithOwner().HasForeignKey("PlanId");
            b.HasKey(d => d.Id);
            b.Property(d => d.Value).HasMaxLength(256);
            b.ToTable("PlanDetails");
        });

        base.OnModelCreating(modelBuilder);
    }
}
