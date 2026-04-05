using System.Text.Json.Serialization;
using PlanIt.Domain.Common.Models;
using PlanIt.Domain.DomainEvents.Schedules;

namespace PlanIt.Domain.Entities;

public class Schedule : Entity<Guid>
{
    [JsonConstructor]
    private Schedule() : base(Guid.NewGuid()) { }

    public static Schedule Create(string name, string description, string location, DateTime startTime, DateTime endTime)
    {
        var schedule = new Schedule
        {
            Name = name,
            Description = description,
            Location = location,
            StartTime = startTime,
            EndTime = endTime,
        };

        schedule.AddDomainEvent(new ScheduleCreatedEvent(schedule));
        return schedule;
    }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }

    public ICollection<Attraction> Attractions { get; private set; } = new List<Attraction>();

    public override void Delete()
    {
        base.Delete();
        AddDomainEvent(new ScheduleDeletedEvent(this));
    }
}