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
        var deleted = await context.Registrants
            .IgnoreQueryFilters()
            .Where(r => r.UserId == userId && r.AttractionId == attractionId)
            .ExecuteDeleteAsync();

        if (deleted == 0)
            throw new NotRegisteredException(userId, attractionId);
    }

    public async Task<ISet<Guid>> GetRegisteredAttractionIds(Guid userId)
    {
        var ids = await context.Registrants
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Select(r => r.AttractionId)
            .ToListAsync();

        return ids.ToHashSet();
    }
}