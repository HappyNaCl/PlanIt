using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.Entities;

public class Attraction : Entity<Guid>
{
    public Attraction() : base(Guid.NewGuid()) { }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageKey { get; set; } = null!;
    public int Capacity { get; set; }

    public Guid ScheduleId { get; init; }
    public Schedule Schedule { get; private set; } = null!;
    public ICollection<Registrant> Registrants { get; private set; } = new List<Registrant>();
}