using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.Entities;

public class Attraction : Entity<Guid>
{
    public Attraction() : base(Guid.NewGuid()) { }

    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string ImageKey { get; init; } = null!;
    public int Capacity { get; init; }

    public Guid ScheduleId { get; init; }
    public Schedule Schedule { get; private set; } = null!;
    public ICollection<Registrant> Registrants { get; private set; } = new List<Registrant>();
}