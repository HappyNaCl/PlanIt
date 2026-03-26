using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.Entities;

public class Schedule : Entity<Guid>
{
    public Schedule() : base(Guid.NewGuid()) { }

    public Schedule(Guid id, string name, DateTime startTime, DateTime endTime)
        : base(id)
    {
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
    }
    
    public required string Name { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }

    public ICollection<Attraction> Attractions { get; init; } = new List<Attraction>();
}