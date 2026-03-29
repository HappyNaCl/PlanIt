using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlanIt.Infrastructure.Persistence;

namespace PlanIt.Infrastructure.Seeder;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await context.Database.EnsureCreatedAsync();
        await context.Database.MigrateAsync();

        await UserSeeder.SeedAsync(context);
        await ScheduleSeeder.SeedAsync(context);
        // await AttractionSeeder.SeedAsync(context);
        // await RegistrantSeeder.SeedAsync(context);
    }
}