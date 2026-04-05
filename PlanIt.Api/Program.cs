using PlanIt.Api;
using PlanIt.Api.Middlewares;
using PlanIt.Application;
using PlanIt.Infrastructure;
using PlanIt.Infrastructure.Realtime;
using PlanIt.Infrastructure.Seeder;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddPresentation();
    builder.Services.AddMappings();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddCorsPolicy();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseCors("AllowFrontend");
    // app.UseAuthenticatiowait n();
    app.UseAuthorization();

    if (args.Contains("--seed"))
        await DatabaseSeeder.SeedAsync(app.Services);

    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapHub<AttractionHub>("/hubs/attractions");
    app.MapHub<AdminHub>("/hubs/admin");
    app.Run();
}