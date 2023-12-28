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

	public bool HasDangerDistance(int trainId, int stationId, double breakingDistance)
	{
		if (trainId <= 0)
		{
			throw new TrainIdIsIncorrectException(nameof(trainId));
		}
		
		if (stationId <= 0)
		{
			throw new StationIdIsIncorrectException(nameof(stationId));
		}

		if (breakingDistance <= 0)
		{
			throw new ArgumentException("Тормозной путь не может быть <= 0", nameof(breakingDistance));
		}

		return _processes
			   .Any(x =>//Существует хотя бы один
					   x.Train.Id != trainId && //Не текущий поезд
					   x.Train.CurrentState.NextStationId == stationId && //едет до следующей станции
					   x.Train.CurrentState.Distance < breakingDistance && //Дистанция меньше тормозного пути
					   x.Train.CurrentState.Distance > 0 //И тот поезд уже тронулся
			   );
	}
}