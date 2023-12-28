using Metro.Data.Contracts.Models.Trains;
using Metro.Services.Processes;
using Metro.Common;
using Metro.Common.Exceptions;
using Metro.Common.Helpers;
using Metro.Services.Contracts.DataServices;
using Metro.Services.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Metro.Services.Factories;

/// <summary>
/// Фабрика поездов
/// </summary>
internal class TrainFactory
{
    private readonly ILineService _lineService;
    private readonly IServiceProvider _serviceProvider;

    public TrainFactory(ILineService lineService, IServiceProvider serviceProvider)
    {
        _lineService = lineService;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Создать поезд
    /// </summary>
    /// <param name="lineId">id линии</param>
    /// <returns>Id поезда</returns>
    public SyncTrainProcess CreateAndStartTrain(int lineId)
    {
	    if (lineId <= 0)
	    {
		    throw new LineIdIsIncorrectException(nameof(lineId));
	    }

		var trainId = IdGenerator.NextId;
        //Создаем поезд, ставим его на линию
        var train = new Train(trainId, Constants.DefaultCarriageCount, lineId);
        train.CreateCarriages();
        _lineService.AddTrain(lineId, train);
        return CreateTrainProcess(train);
    }

    /// <summary>
    /// Создать процесс управления поездом
    /// </summary>
    /// <param name="train">поезд</param>
    /// <returns></returns>
    /// <exception cref="TrainIsNullException"></exception>
    public SyncTrainProcess CreateTrainProcess(Train train)
    {
	    if (train ==null)
	    {
		    throw new TrainIsNullException(nameof(train));
	    }

		//Получаем процесс управления
		var trainProcess = _serviceProvider.GetRequiredService<SyncTrainProcess>();
        trainProcess.InitTrainProcess(train);
        return trainProcess;
    }
}