using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.Entities;

public class Registrant : AggregateRoot<Guid>
{
    public Registrant() : base(Guid.NewGuid()) { }

    public Registrant(Guid id, Guid userId, Guid attractionId) : base(id)
    {
        UserId = userId;
        AttractionId = attractionId;
    }
    
    public Guid UserId { get; init; }
    public Guid AttractionId { get; init; }

    public User User { get; private set; } = null!;
    public Attraction Attraction { get; private set; } = null!;
}