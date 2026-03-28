using Microsoft.EntityFrameworkCore;
using Npgsql;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Common.Exceptions.Users;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.Persistence;

public class UserRepository(IApplicationDbContext context) : IUserRepository
{
    public async Task<User> Create(User user)
    {
        context.Users.Add(user);

        try
        {
            await context.SaveChangesAsync(CancellationToken.None);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pge)
        {
            if (pge.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                throw pge.ConstraintName switch
                {
                    "IX_Users_Email"    => new UserDuplicateEmailException(),
                    "IX_Users_Username" => new UserDuplicateUsernameException(),
                    _ => new InvalidOperationException($"Unhandled unique constraint: {pge.ConstraintName}"),
                };
            }
            throw;
        }
        
        return user;
    }

    public async Task<User> GetById(Guid id)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return user ?? throw new UserNotFoundException(id.ToString());
    }

    public async Task<User> GetByUsername(string username)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);

        return user ?? throw new UserNotFoundException(username);
    }

    public async Task<User?> GetByUsernameDefault(string username)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);

        return user;
    }
}