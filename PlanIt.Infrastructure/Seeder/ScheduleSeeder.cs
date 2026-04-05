using Bogus;
using Microsoft.EntityFrameworkCore;
using PlanIt.Domain.Entities;
using PlanIt.Infrastructure.Persistence;

namespace PlanIt.Infrastructure.Seeder;

public static class ScheduleSeeder
{
    private const int ScheduleCount = 50;

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.Schedules.AnyAsync())
        {
            var faker = new Faker<Schedule>()
                .CustomInstantiator(f =>
                {
                    var startTime = DateTime.SpecifyKind(
                        f.Date.Between(DateTime.UtcNow, DateTime.UtcNow.AddMonths(1)).Date.AddHours(f.Random.Int(8, 18)),
                        DateTimeKind.Utc
                    );
                    var maxDuration = 23 - startTime.Hour;
                    var endTime = DateTime.SpecifyKind(
                        startTime.AddHours(f.Random.Int(1, maxDuration)),
                        DateTimeKind.Utc
                    );

                    return Schedule.Create(
                        f.Commerce.ProductName(),
                        f.Lorem.Sentence(),
                        $"{f.Address.City()}, {f.Address.Country()}",
                        startTime,
                        endTime);
                });

            var schedules = faker.Generate(ScheduleCount);
            schedules.ForEach(s => s.ClearDomainEvents());

            context.Schedules.AddRange(schedules);
            await context.SaveChangesAsync();
        }
    }
}