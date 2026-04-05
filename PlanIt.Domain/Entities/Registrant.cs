using PlanIt.Domain.Common.Models;
using PlanIt.Domain.DomainEvents.Registrants;

namespace PlanIt.Domain.Entities;

public class Registrant : AggregateRoot<Guid>
{
    private Registrant() : base(Guid.NewGuid()) { }

    public static Registrant Create(Guid userId, Guid attractionId)
    {
        var registrant = new Registrant
        {
            UserId = userId,
            AttractionId = attractionId,
        };

        registrant.AddDomainEvent(new RegistrantCreatedEvent(registrant));
        return registrant;
    }

    public Guid UserId { get; init; }
    public Guid AttractionId { get; init; }

    public User User { get; private set; } = null!;
    public Attraction Attraction { get; private set; } = null!;

    public void Leave()
    {
        AddDomainEvent(new RegistrantDeletedEvent(this));
    }
}