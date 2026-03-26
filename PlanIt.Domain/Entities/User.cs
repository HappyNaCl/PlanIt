using PlanIt.Domain.Common.Enums;
using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.Entities;

public class User : Entity<Guid>
{
    public User() : base(Guid.NewGuid())
    {
    }

    public User(Guid id, string username, string email, string password, UserRole role = UserRole.USER)
        : base(id)
    {
        Username = username;
        Email = email;
        Password = password;
        Role = role;
    }
    
    public required string Username { get; init; }
    public required string Email { get; init; } 
    public required string Password { get; init; }
    public required UserRole Role { get; init; } = UserRole.USER;
    
    public ICollection<Registrant> RegisteredAttractions { get; init; } = new List<Registrant>();
}