// See https://aka.ms/new-console-template for more information


using Metro.Bootstrap;
using Microsoft.Extensions.Hosting;

await new HostBuilder()
	.ConfigureServices((hostContext, services) =>
    {
        services.AddMetro();
    })
	.RunConsoleAsync();

Console.ReadKey();