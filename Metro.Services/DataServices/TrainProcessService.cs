using Metro.Common.Exceptions;
using Metro.Data.Contracts;
using Metro.Data.Contracts.Models.Trains;
using Metro.Services.Contracts.DataServices;
using Metro.Services.Factories;
using Metro.Services.Processes;

namespace Metro.Services.DataServices;

internal class TrainProcessService : ITrainProcessService
{
	/// <summary>
	/// Список процессов управления поездами
	/// </summary>
	private readonly List<SyncTrainProcess> _processes;
	private readonly IDataSource _dataSource;
	private readonly TrainFactory _factory;

	public TrainProcessService(TrainFactory factory, IDataSource dataSource)
	{
		_factory = factory;
		_dataSource = dataSource;
		_processes = new List<SyncTrainProcess>();
	}

	public int StartNewTrain(int lineId)
	{
		if (lineId <= 0)
		{
			throw new LineIdIsIncorrectException(nameof(lineId));
		}

		var process = _factory.CreateAndStartTrain(lineId);
		_processes.Add(process);
		return process.TrainId;
	}

	public int LoadTrainProcess(Train train)
	{
		if (train == null)
		{
			throw new TrainIsNullException(nameof(train));
		}

		var process = _factory.CreateTrainProcess(train);
		_processes.Add(process);
		return process.TrainId;
	}

	public void Reset()
	{
		_processes.Clear();
		_dataSource.Reset();
	}

	public void Tick()
	{
		//Подпвинуть все поезда
		foreach (var trainProcess in _processes)
		{
			trainProcess.Processed();
		}
	}
}