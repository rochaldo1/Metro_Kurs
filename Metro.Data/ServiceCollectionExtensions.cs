using Metro.Data.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Metro.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMetroDataServices(this IServiceCollection services)
    {
        services.AddSingleton<IDataSource, DataSource>(_ =>
        {
            var dataSource = new DataSource();
            dataSource.LoadLinesData();
            return dataSource;
        });

        return services;
    }
}
