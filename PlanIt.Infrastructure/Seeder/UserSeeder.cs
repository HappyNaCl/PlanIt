using Bogus;
using Microsoft.EntityFrameworkCore;
using PlanIt.Domain.Common.Enums;
using PlanIt.Domain.Entities;
using PlanIt.Infrastructure.Authentication;
using PlanIt.Infrastructure.Persistence;

namespace PlanIt.Infrastructure.Seeder;

public static class UserSeeder
{
    private const int UserCount = 100;

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.Users.AnyAsync(u => u.Role == UserRole.ADMIN))
        {
            var hasher = new BCryptPasswordHasher();

            var adminUser = User.Create("admin", "admin@gmail.com", hasher.Hash("adminadmin"), UserRole.ADMIN);
            adminUser.ClearDomainEvents();

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync(u => u.Role == UserRole.USER))
        {
            var hasher = new BCryptPasswordHasher();
            var hashedPassword = hasher.Hash("useruser");

            var faker = new Faker<User>()
                .CustomInstantiator(f => User.Create(f.Internet.UserName(), f.Internet.Email(), hashedPassword));

            var users = faker.Generate(UserCount);
            users.ForEach(u => u.ClearDomainEvents());

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
}