using Metro.Services.Contracts.DataServices;
using Metro.Services.DataServices;
using Metro.Services.Factories;
using Metro.Services.Processes;
using Microsoft.Extensions.DependencyInjection;

namespace Metro.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMetroServices(this IServiceCollection services)
	{
		return services.AddTransient<SyncTrainProcess>()
			.AddTransient<TrainFactory>()
			.AddTransient<IPassengersFactory, PassengersFactory>()
			.AddTransient<ILineService, LineService>()
			.AddSingleton<ITrainProcessService, TrainProcessService>()
			.AddSingleton<IAnalysisService, AnalysisService>();
	}
}
