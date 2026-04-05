using System.Text.Json.Serialization;
using PlanIt.Domain.Common.Enums;
using PlanIt.Domain.Common.Models;
using PlanIt.Domain.DomainEvents.Users;

namespace PlanIt.Domain.Entities;

public class User : Entity<Guid>
{
    [JsonConstructor]
    private User() : base(Guid.NewGuid()) { }

    public static User Create(string username, string email, string password, UserRole role = UserRole.USER)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            Password = password,
            Role = role,
        };

        user.AddDomainEvent(new UserRegisteredEvent(user));
        return user;
    }

    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public UserRole Role { get; init; } = UserRole.USER;

    public ICollection<Registrant> RegisteredAttractions { get; private set; } = new List<Registrant>();
}