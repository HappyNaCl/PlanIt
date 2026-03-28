using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.Entities;

public class Attraction : Entity<Guid>
{
    public Attraction() : base(Guid.NewGuid()) { }
    
    public Attraction(Guid id, string name, string description, string imageUrl, int capacity, Guid scheduleId)
        : base(id)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        Capacity = capacity;
        ScheduleId = scheduleId;
    }
    
    public string Name { get; init; }
    public string Description { get; init; }
    public string ImageUrl { get; init; }
    public int Capacity { get; init; }
    
    public Guid ScheduleId { get; private set; }
    public Schedule Schedule { get; private set; } = null!;
    public ICollection<Registrant> Registrants { get; private set; } = new List<Registrant>();

    public void Remove()
    {
        // AddDomainEvent(new AttractionDeletedEvent(ScheduleId, Id));
    }

    public void Update()
    {
        // AddDomainEvent(new AttractionUpdatedEvent(this, ScheduleId));
    }
}