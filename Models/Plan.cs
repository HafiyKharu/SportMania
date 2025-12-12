using SportMania.Models.Interface;

namespace SportMania.Models;

public class Plan : IHasAuditTimestamps
{
    public Guid PlanId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Duration { get; set; } = string.Empty;
    public List<Details> Details { get; set; } = new List<Details>();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
    
}

public class Details
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
}