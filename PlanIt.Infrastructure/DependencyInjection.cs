using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Datetime;
using PlanIt.Infrastructure.Authentication;
using PlanIt.Infrastructure.Datetime;
using PlanIt.Infrastructure.FileUploader;

namespace PlanIt.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        
        services.AddSingleton<IDatetimeProvider, DatetimeProvider>();
        
        services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));
        services.Configure<S3Settings>(config.GetSection(S3Settings.SectionName));


        services.AddSingleton<IAmazonS3>(sp =>
            {
                var s3Config = config.GetSection("S3");

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