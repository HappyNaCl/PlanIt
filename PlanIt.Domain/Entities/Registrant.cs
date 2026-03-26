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
    
    public required Guid UserId { get; init; }
    public required Guid AttractionId { get; init; }

    public User User { get; set; } = null!;
    public Attraction Attraction { get; set; } = null!;
}