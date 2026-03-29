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

            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@gmail.com",
                Password = hasher.Hash("adminadmin"),
                Role = UserRole.ADMIN
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync(u => u.Role == UserRole.USER))
        {
            var hasher = new BCryptPasswordHasher();
            var hashedPassword = hasher.Hash("useruser");

            var faker = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    Username = f.Internet.UserName(),
                    Email = f.Internet.Email(),
                    Password = hashedPassword,
                    Role = UserRole.USER
                });

            var users = faker.Generate(UserCount);

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
}