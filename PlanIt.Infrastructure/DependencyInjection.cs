using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Datetime;
using PlanIt.Application.Common.Interfaces.FileUploader;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Common.Interfaces.Locking;
using PlanIt.Application.Common.Interfaces.Messaging;
using PlanIt.Application.Common.Interfaces.Realtime;
using PlanIt.Application.Common.Interfaces.Stores;
using PlanIt.Infrastructure.Authentication;
using PlanIt.Infrastructure.CachedPersistence;
using PlanIt.Infrastructure.Datetime;
using PlanIt.Infrastructure.FileUploader;
using PlanIt.Infrastructure.Locking;
using PlanIt.Infrastructure.Messaging;
using PlanIt.Infrastructure.Messaging.Consumers;
using PlanIt.Infrastructure.Realtime;
using RabbitMQ.Client;
using PlanIt.Infrastructure.Persistence;
using PlanIt.Infrastructure.Stores;
using StackExchange.Redis;

namespace PlanIt.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());
        
        services.AddStackExchangeRedisCache(options =>
            options.Configuration = config.GetConnectionString("Redis"));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(config.GetConnectionString("Redis")!));
        services.AddSingleton<IDatabase>(sp =>
            sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(sp =>
            new CachedUserRepository(
                sp.GetRequiredService<UserRepository>(),
                sp.GetRequiredService<IDistributedCache>()
            )
        );
        services.AddScoped<ScheduleRepository>();
        services.AddScoped<IScheduleRepository>(sp =>
            new CachedScheduleRepository(
                sp.GetRequiredService<ScheduleRepository>(),
                sp.GetRequiredService<IDistributedCache>()
            )
        );
        services.AddScoped<AttractionRepository>();
        services.AddScoped<IAttractionRepository>(sp =>
            new CachedAttractionRepository(
                sp.GetRequiredService<AttractionRepository>(),
                sp.GetRequiredService<IDatabase>()
            )
        );

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<RegistrantRepository>();
        services.AddScoped<IRegistrantRepository>(sp =>
            new CachedRegistrantRepository(
                sp.GetRequiredService<RegistrantRepository>(),
                sp.GetRequiredService<IDatabase>()
            )
        );
        services.AddScoped<IIdempotencyStore<string>, RedisIdempotencyStore>();
        services.AddSingleton<IDistributedLock, RedisDistributedLock>();

        services.Configure<RabbitMqSettings>(config.GetSection(RabbitMqSettings.SectionName));
        services.AddSingleton<IConnection>(_ =>
        {
            var rabbitConfig = config.GetSection(RabbitMqSettings.SectionName);
            var factory = new ConnectionFactory
            {
                HostName = rabbitConfig["Host"] ?? "localhost",
                UserName = rabbitConfig["Username"] ?? "guest",
                Password = rabbitConfig["Password"] ?? "guest"
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });
        services.AddSingleton<IEventBus, RabbitMqEventBus>();
        services.AddSingleton<IAttractionNotifier, AttractionNotifier>();
        services.AddSingleton<IAdminNotifier, AdminNotifier>();
        services.AddHostedService<JoinAttractionConsumer>();

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<AccessTokenService>();
        services.AddScoped<IAccessTokenGenerator>(sp => sp.GetRequiredService<AccessTokenService>());
        services.AddScoped<IAccessTokenValidator>(sp => sp.GetRequiredService<AccessTokenService>());
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<IRefreshTokenGenerator>(sp => sp.GetRequiredService<RefreshTokenService>());
        services.AddScoped<IRefreshTokenValidator>(sp => sp.GetRequiredService<RefreshTokenService>());
        
        services.AddSingleton<IDatetimeProvider, DatetimeProvider>();
        services.AddSingleton<IFileUploader, S3Uploader>();

        services.Configure<TokenSettings>(config.GetSection(TokenSettings.SectionName));
        services.Configure<S3Settings>(config.GetSection(S3Settings.SectionName));
        
        services.AddSingleton<IAmazonS3>(_ =>
            {
                var s3Config = config.GetSection(S3Settings.SectionName);
                return new AmazonS3Client(
                    new BasicAWSCredentials(
                        s3Config["AccessKey"],
                        s3Config["SecretKey"]
                    ),
                    new AmazonS3Config
                    {
                        ServiceURL = s3Config["Endpoint"],
                        ForcePathStyle = true
                    }
                );
            }
        );
        
        return services;
    }
}