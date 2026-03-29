using PlanIt.Domain.Common.Enums;
using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.Entities;

public class User : Entity<Guid>
{
    public User() : base(Guid.NewGuid()) { }

    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public UserRole Role { get; init; } = UserRole.USER;

    public ICollection<Registrant> RegisteredAttractions { get; private set; } = new List<Registrant>();
}