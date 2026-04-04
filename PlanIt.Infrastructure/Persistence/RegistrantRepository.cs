using Microsoft.EntityFrameworkCore;
using Npgsql;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Common.Exceptions.Registrants;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.Persistence;

public class RegistrantRepository(IApplicationDbContext context) : IRegistrantRepository
{
    public async Task<Registrant> AddAsync(Registrant registrant)
    {
        try
        {
            context.Registrants.Add(registrant);
            await context.SaveChangesAsync(CancellationToken.None);
            return registrant;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            throw new AlreadyRegisteredException(registrant.UserId, registrant.AttractionId);
        }
    }

    public async Task RemoveAsync(Guid userId, Guid attractionId)
    {
        var registrant = await context.Registrants
            .FirstOrDefaultAsync(r => r.UserId == userId && r.AttractionId == attractionId)
            ?? throw new NotRegisteredException(userId, attractionId);

        context.Registrants.Remove(registrant);
        await context.SaveChangesAsync(CancellationToken.None);
    }
}