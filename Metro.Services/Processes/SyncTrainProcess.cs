using Metro.Common;
using Metro.Data.Contracts.Enums;
using Metro.Data.Contracts.Models.Lines;
using Metro.Data.Contracts.Models.Trains;
using Metro.Services.Contracts.DataServices;
using Metro.Services.Extensions;

namespace Metro.Services.Processes;

internal class SyncTrainProcess
{
	/// <summary>
	/// Id поезда
	/// </summary>
	public int TrainId => Train.Id;

	/// <summary>
	/// Поезд, которым управляем
	/// </summary>
	public Train Train { set; get; }

	/// <summary>
	/// Состояние поезда (Чтобы не писать Train.CurrentState - просто State)
	/// </summary>
	private TrainState State => Train.CurrentState;

	private int _waitForDepot;
	private int _waitOnStation;

	/// <summary>
	/// Тормозной путь, расчитается как только поезд будет добавлен
	/// </summary>
	private double _breakingDistance;

	private readonly ILineService _lineService;
	private readonly IAnalysisService _analysisService;

	public SyncTrainProcess(ILineService lineService, IAnalysisService analysisService)
	{
		_lineService = lineService;
		_analysisService = analysisService;
	}

	/// <summary>
	/// Запустить созданный поезд в процесс
	/// </summary>
	/// <param name="train"></param>
	/// <returns></returns>
	public void InitTrainProcess(Train train)
	{
		if (train == null)
		{
			throw new ArgumentNullException(nameof(train), "Поезд не может быть null");
		}

		Train = train;
		//Как только получили поезд из очереди, считаем и сохраняем его тормозной путь
		_breakingDistance = GetBreakingDistance();
	}

	/// <summary>
	/// Управлять процессом поезда
	/// </summary>
	public void Processed()
	{
		switch (State.Status)
		{
			case ETrainStatus.Depot:
				DepotProcess();
				break;
			case ETrainStatus.Speeding:
				PickUpSpeed();
				break;
			case ETrainStatus.Rides:
				HoldSpeed();
				break;
			case ETrainStatus.Slowing:
				SlowDownSpeed();
				break;
			case ETrainStatus.Station:
				WaitOnStation();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

	}

	/// <summary>
	/// Обработка процессов в депо
	/// </summary>
	private void DepotProcess()
	{
		if (State.NextStationId == 0)
		{
			SetNextStation();
		}

		var line = _lineService.GetLine(Train.LineId);
		var station = _lineService.GetStation(Train.LineId, State.CurrentStationId.Value);

		
		if (!station.IsWork() && Train.PassengersCount == 0 &&
			!Core.IsWorkTime() && line.PassengersCount == 0)
		{
			return;
		}

		//Если на станции есть поезд
		if (station.CurrentTrainId.HasValue)
		{
			//И этот поезд не текущий то просто выход и продолжаем ждать в депо
			if (station.CurrentTrainId != TrainId)
			{
				_waitForDepot = 0;
				return;
			}

			//Иначе мы едем на станцию
			//Но сначала мы должны подождать паузу
			if (_waitForDepot < Constants.PauseOnStationAfterDepot)
			{
				_waitForDepot++;
				return;
			}

			_waitForDepot = 0;
			State.Status = ETrainStatus.Station;
			station.CurrentTrainId = TrainId;
		}
		else
		{
			//Если на станции нет поезда, попадаем сразу на станцию?
			if (_waitForDepot < Constants.PauseOnStationAfterDepot)
			{
				_waitForDepot++;
				return;
			}

			_waitForDepot = 0;
			State.Status = ETrainStatus.Station;
			station.CurrentTrainId = TrainId;
		}
	}

	/// <summary>
	/// Получить томозной путь поезда
	/// </summary>
	/// <returns></returns>
	private static double GetBreakingDistance()
	{
		var result = 0.0;
		var v0 = Constants.MaxTrainSpeed;
		var speed = Constants.MaxTrainSpeed;
		while (speed > 0)
		{
			speed -= Constants.AccelerationSlowDown;
			//Сколько проехал за секунду на текущей скорости
			var d = v0 + Constants.AccelerationSlowDown * 1 / 2;
			//Общая дистанция
			result += d;
			v0 = speed;
		}

		return result;
	}

	/// <summary>
	/// Набор скорости
	/// </summary>
	private void PickUpSpeed()
	{
		if (State.Speed <= Constants.MaxTrainSpeed)
		{
			State.Status = ETrainStatus.Speeding;
			//Увеличить скорость на скорость м/с, которую набрали за секунду
			State.Speed += Constants.AccelerationSpeedUp;
			//Дистанция, пройденная за секунду на текущих скоростях
			var distance = State.Speed + Constants.AccelerationSpeedUp * 1 / 2;
			State.Distance += distance;
			State.TotalDistance += distance;
		}
		else
		{
			State.Status = ETrainStatus.Rides;
		}
	}

	/// <summary>
	/// Снижение скорости
	/// </summary>
	private void SlowDownSpeed()
	{
		if (State.Speed > 0)
		{
			//Увеличить скорость на скорость м/с, которую набрали за секунду
			State.Speed -= Constants.AccelerationSlowDown;
			//Дистанция, пройденная за секунду на текущих скоростях
			var distance = State.Speed + Constants.AccelerationSlowDown * 1 / 2;
			State.Distance += distance;
			State.TotalDistance += distance;
		}
		else
		{
			State.Speed = 0;
			State.CurrentStationId = State.NextStationId;
			State.Status = ETrainStatus.Station;
		}
	}

	/// <summary>
	/// Ожидание на станции
	/// </summary>
	/// <returns></returns>
	private void WaitOnStation()
	{
		//Найти станцию, на которой находится поезд
		var station = _lineService.GetStation(Train.LineId, State.CurrentStationId.Value);

		State.Status = ETrainStatus.Station;
		station.CurrentTrainId = Train.Id;
		//Поезд ждет на станции
		var waitTime = station.StationWaitTimes.GetCurrentCounter();

		if (waitTime != 0) //станция работает - ждём на станции
		{
			if (waitTime > _waitOnStation)
			{
				if (_waitOnStation == 0)
				{
					UnloadPassengers();
				}
				_waitOnStation++;
				return;
			}
		}
		else if(_waitOnStation == 0)
		{
			UnloadPassengers();
		}

		var line = _lineService.GetLine(Train.LineId);
		if (State.CurrentStationId == line.Stations[0].Id && State.Direction == ETrainDirection.Backward)
		{
			//Поезд приехал на начальную (конечную станцию). Заехать в депо и стартовать заново
			TrainEndingToFirstStation();
			return;
		}

		//Поезд отправляется со станции, если сможет
		if (!_isSetNextStation)
		{
			_isSetNextStation = true;
			SetNextStation();
		}

		LoadPassengers();

		var nextStation = _lineService.GetStation(line.Id, State.NextStationId);
		if (nextStation.CurrentTrainId.HasValue)
		{
			var nextTrain = line.Trains.FirstOrDefault(x => x.Id == nextStation.CurrentTrainId.Value);
			if (nextTrain != null && nextTrain.CurrentState.Direction == State.Direction)
				return;
		}

		if (line.Trains.Any(x => x.CurrentState.NextStationId == nextStation.Id && x.Id != TrainId && x.CurrentState.Status != ETrainStatus.Depot && x.CurrentState.Direction == State.Direction))
		{
			return;
		}

		station.CurrentTrainId = null;
		_waitOnStation = 0;
		State.Status = ETrainStatus.Speeding;
		_isSetNextStation = false;
	}
	
	/// <summary>
	/// Поставить поезд в депо
	/// </summary>
	private void TrainEndingToFirstStation()
	{
		UnloadPassengers();

		State.CurrentStationId = null;
		State.Status = ETrainStatus.Depot;
		State.Distance = 0;
		State.DistanceToNextStation = 0;
		State.Direction = ETrainDirection.Forward;
		State.Speed = 0;
		State.NextStationName = null;
		State.TotalDistance = 0;
		State.NextStationId = 0;
	}

	private bool _isSetNextStation = false;

	/// <summary>
	/// Проверка на то, нужна ли остановка
	/// </summary>
	/// <returns>
	/// True - нужно начинать останавливаться
	/// False - можно продолжать путь
	/// </returns>
	private bool NeedToStop()
	{
		var needToStop = State.DistanceToNextStation - State.Distance <= _breakingDistance;

		//Проверить, нужно ли тормозить на следующей станции поезду
		//Найти станцию, куда едет поезд
		var station = _lineService.GetStation(Train.LineId, State.NextStationId);
		if (station.StationWaitTimes.GetCurrentCounter() == 0)
		{
			//Станция, на которую едет поезд - 
		}

		return needToStop;
	}

	/// <summary>
	/// В пути (одинаковая скорость)
	/// </summary>
	private void HoldSpeed()
	{
		if (!NeedToStop())
		{
			State.Distance += State.Speed;
			State.TotalDistance += State.Speed;
		}
		else
		{
			State.Status = ETrainStatus.Slowing;
		}
	}

	/// <summary>
	/// Выгрузка пассажиров из поезда
	/// </summary>
	private void UnloadPassengers()
	{
		_lineService.UnloadPassengersFromTrain(Train, State.CurrentStationId.Value);
	}

	/// <summary>
	/// Загрузка пассажиров в поезд (вагоны)
	/// </summary>
	private void LoadPassengers()
	{
		var passengers = _lineService.GetPassengersToGo(Train.LineId, State.Direction, State.CurrentStationId.Value);
		foreach (var passenger in passengers)
		{
			var carriage = Train.GetFreeCarriageForPassenger();
			if (carriage == null)
				break;

			carriage.Passengers.Add(passenger.Value);
			_lineService.StationRemovePassenger(State.CurrentStationId.Value, passenger.Value);

			passenger.Value.EndWaitTime = Core.Time;
			_analysisService.AddPassengerWaitTime(passenger.Value.StartWaitTime, passenger.Value.EndWaitTime);
		}
	}

	/// <summary>
	/// Поиск и установка следующей станции
	/// </summary>
	private void SetNextStation()
	{
		var line = _lineService.GetLine(Train.LineId);

		Station station;

		if (!State.CurrentStationId.HasValue)
		{
			station = line.Stations.First();

			State.CurrentStationId = station.Id;
			State.DistanceToNextStation = station.Distance;
		}

		var stationId = State.CurrentStationId.Value;

		//Если едем вперед, то пробуем взять станцию следубщую
		if (State.Direction == ETrainDirection.Forward)
		{
			station = line.Stations.GetNextStation(stationId);
			if (station == null)
			{
				//Если следующая станция не найдена - поворачиваем назад
				State.Direction = ETrainDirection.Backward;
				State.TotalDistance = 0;
				SetNextStation();
				return;
			}
		}
		else
		{
			station = line.Stations.GetPrevStation(stationId);
			if (station == null)
			{
				//Если следующая станция не найдена - поворачиваем вперед
				State.Direction = ETrainDirection.Forward;
				SetNextStation();
				return;
			}
		}

		State.NextStationId = station.Id;
		State.NextStationName = station.Name;
		if (State.Direction == ETrainDirection.Forward)
		{
			State.DistanceToNextStation = line.Stations.First(x => x.Id == State.CurrentStationId.Value).Distance;
		}
		else
		{
			State.DistanceToNextStation = station.Distance;
		}

		State.Distance = 0;
	}
}