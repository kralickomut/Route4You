using Route4You.Application.Abstraction.Areas;
using Route4You.Application.Abstraction.Ascents;
using Route4You.Application.Abstraction.Images;
using Route4You.Application.Abstraction.Routes;
using Route4You.Application.Abstraction.Users;
using Route4You.Application.Services;
using Route4You.Infrastructure;
using Route4You.Infrastructure.Data;
using Route4You.Infrastructure.Repositories;
using Route4You.Infrastructure.Repositories.Areas;
using Route4You.Infrastructure.Repositories.Ascents;
using Route4You.Infrastructure.Repositories.Routes;
using Route4You.Infrastructure.Repositories.Users;
using Route4You.Infrastructure.Storage;

namespace Route4You.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoOptions>(configuration.GetSection("Mongo"));
        services.AddSingleton<MongoContext>();

        
        
        services.AddHostedService<Initializer>(); // checks connection with a db
        
        services.AddScoped<IAscentRepository, AscentRepository>();
        services.AddScoped<IAscentService, AscentService>();
        
        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<IAreaService, AreaService>();

        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<IRouteRepository, RouteRepository>();
        
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.Configure<BlobStorageOptions>(configuration.GetSection("BlobStorage"));
        services.AddScoped<IImageStorage, AzureBlobImageStorage>();
        
        
        return services;
    }
}
