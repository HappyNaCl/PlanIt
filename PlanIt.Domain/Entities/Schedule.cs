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
    
    public string Name { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }

    public ICollection<Attraction> Attractions { get; private set; } = new List<Attraction>();
}