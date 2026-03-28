using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Datetime;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Infrastructure.Authentication;
using PlanIt.Infrastructure.CachedPersistence;
using PlanIt.Infrastructure.Datetime;
using PlanIt.Infrastructure.FileUploader;
using PlanIt.Infrastructure.Persistence;

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

        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(sp =>
            new CachedUserRepository(
                sp.GetRequiredService<UserRepository>(),
                sp.GetRequiredService<IDistributedCache>()
            ));
        
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<AccessTokenService>();
        services.AddScoped<IAccessTokenGenerator>(sp => sp.GetRequiredService<AccessTokenService>());
        services.AddScoped<IAccessTokenValidator>(sp => sp.GetRequiredService<AccessTokenService>());
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<IRefreshTokenGenerator>(sp => sp.GetRequiredService<RefreshTokenService>());
        services.AddScoped<IRefreshTokenValidator>(sp => sp.GetRequiredService<RefreshTokenService>());
        
        services.AddSingleton<IDatetimeProvider, DatetimeProvider>();
        
        services.Configure<TokenSettings>(config.GetSection(TokenSettings.SectionName));
        
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