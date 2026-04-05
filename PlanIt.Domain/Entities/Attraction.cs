using System.Text.Json.Serialization;
using PlanIt.Domain.Common.Models;
using PlanIt.Domain.DomainEvents.Attractions;

namespace PlanIt.Domain.Entities;

public class Attraction : Entity<Guid>
{
    [JsonConstructor]
    private Attraction() : base(Guid.NewGuid()) { }

    public static Attraction Create(string name, string description, string imageKey, int capacity, Guid scheduleId)
    {
        var attraction = new Attraction
        {
            Name = name,
            Description = description,
            ImageKey = imageKey,
            Capacity = capacity,
            ScheduleId = scheduleId,
        };

        attraction.AddDomainEvent(new AttractionCreatedEvent(attraction));
        return attraction;
    }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageKey { get; set; } = null!;
    public int Capacity { get; set; }

    public Guid ScheduleId { get; init; }
    public Schedule Schedule { get; private set; } = null!;
    public ICollection<Registrant> Registrants { get; private set; } = new List<Registrant>();

    public override void Delete()
    {
        base.Delete();
        AddDomainEvent(new AttractionDeletedEvent(this));
    }
}