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

        // --- Seed Plans + Details ---
        var weeklyId  = Guid.Parse("3f2c8d3e-7f22-4e54-9c2b-4a8b9d5a1a11");
        var monthlyId = Guid.Parse("0a9c7f12-1b23-4c45-8d67-1e2f3a4b5c66");
        var seasonId  = Guid.Parse("9b8a7c6d-5e4f-4a3b-9c2d-1a0b9c8d7e6f");

        var seededAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Plan>().HasData(
            new Plan
            {
                PlanId = weeklyId,
                ImageUrl = "/Media/BatteryBar.png",
                Name = "Weekly Pass",
                Description = "Access all content for 7 days",
                Price = "RM3.00",
                Duration = "week",
                CreatedAt = seededAt,
                UpdatedAt = null,
                DeletedAt = null,
                IsDeleted = false
            },
            new Plan
            {
                PlanId = monthlyId,
                ImageUrl = "/Media/Star.jpeg",
                Name = "Monthly Plan",
                Description = "Full access for 30 days",
                Price = "RM10.00",
                Duration = "month",
                CreatedAt = seededAt,
                UpdatedAt = null,
                DeletedAt = null,
                IsDeleted = false
            },
            new Plan
            {
                PlanId = seasonId,
                ImageUrl = "/Media/crown.png",
                Name = "Season Pass",
                Description = "Premium access for the entire season",
                Price = "RM60.00",
                Duration = "Full season access",
                CreatedAt = seededAt,
                UpdatedAt = null,
                DeletedAt = null,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Plan>().OwnsMany(p => p.Details).HasData(
            // Weekly details
            new { Id = Guid.Parse("c1b9d1c0-2d4d-4d3b-9f3e-b5bd6f5f0001"), PlanId = weeklyId,  Value = "7 days of full access" },
            new { Id = Guid.Parse("c1b9d1c0-2d4d-4d3b-9f3e-b5bd6f5f0002"), PlanId = weeklyId,  Value = "HD streaming" },
            new { Id = Guid.Parse("c1b9d1c0-2d4d-4d3b-9f3e-b5bd6f5f0003"), PlanId = weeklyId,  Value = "Cancel anytime" },

            // Monthly details (fixed GUIDs)
            new { Id = Guid.Parse("d2c0e2d1-3e5e-4e4c-af4f-c6ce7e6e0001"), PlanId = monthlyId, Value = "30 days of full access" },
            new { Id = Guid.Parse("d2c0e2d1-3e5e-4e4c-af4f-c6ce7e6e0002"), PlanId = monthlyId, Value = "HD & 4K streaming" },
            new { Id = Guid.Parse("d2c0e2d1-3e5e-4e4c-af4f-c6ce7e6e0003"), PlanId = monthlyId, Value = "Download for offline" },

            // Season details (fixed GUIDs)
            new { Id = Guid.Parse("e3d1f3e2-4f6f-5f5d-b05a-d7df8e7e0001"), PlanId = seasonId,  Value = "4K Ultra HD" },
            new { Id = Guid.Parse("e3d1f3e2-4f6f-5f5d-b05a-d7df8e7e0002"), PlanId = seasonId,  Value = "Priority support" },
            new { Id = Guid.Parse("e3d1f3e2-4f6f-5f5d-b05a-d7df8e7e0003"), PlanId = seasonId,  Value = "Exclusive content" },
            new { Id = Guid.Parse("e3d1f3e2-4f6f-5f5d-b05a-d7df8e7e0004"), PlanId = seasonId,  Value = "Early releases" }
        );

        base.OnModelCreating(modelBuilder);
    }
}
