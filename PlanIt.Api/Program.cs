using System.Text.Json.Serialization;
using PlanIt.Api;
using PlanIt.Api.Middlewares;
using PlanIt.Application;
using PlanIt.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
            options.SuppressModelStateInvalidFilter = true)
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull);
    builder.Services.AddMappings();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    // Middlewares
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
    
}