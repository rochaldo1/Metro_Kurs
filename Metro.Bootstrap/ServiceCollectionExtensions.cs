using Metro.Data;
using Metro.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Metro.Bootstrap;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMetro(this IServiceCollection services)
    {
        services.AddMetroDataServices();
        services.AddMetroServices();
        return services;
    }
}
