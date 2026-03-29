using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.Entities;

public class Schedule : Entity<Guid>
{
    public Schedule() : base(Guid.NewGuid()) { }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }

    public ICollection<Attraction> Attractions { get; private set; } = new List<Attraction>();
}
