using Metro.Common;
using Metro.Common.Exceptions;
using Metro.Data.Contracts;
using Metro.Data.Contracts.Enums;
using Metro.Data.Contracts.Models.Lines;
using Metro.Data.Contracts.Models.Passengers;
using Metro.Data.Contracts.Models.Trains;
using Metro.Services.Contracts.DataServices;
using Metro.Services.Extensions;

namespace Metro.Services.DataServices;

internal class LineService : ILineService
{
	private readonly IDataSource _dataSource;

	public LineService(IDataSource dataSource)
	{
		_dataSource = dataSource;
	}

	public MetroLine GetLine(int id)
	{
		if (id <= 0)
		{
			throw new LineIdIsIncorrectException(nameof(id));
		}

		return _dataSource.Lines.First(x => x.Id == id);
	}

	public List<MetroLine> GetLines()
	{
		return _dataSource.Lines;
	}

	public Station GetStation(int lineId, int stationId)
	{
		if (lineId <= 0)
		{
			throw new LineIdIsIncorrectException(nameof(lineId));
		}

		if (stationId <= 0)
		{
			throw new StationIdIsIncorrectException(nameof(stationId));
		}

		return GetLine(lineId).Stations.FirstOrDefault(x => x.Id == stationId);
	}

	public Station GetStation(int stationId)
	{
		if (stationId <= 0)
		{
			throw new StationIdIsIncorrectException(nameof(stationId));
		}

		foreach (var line in _dataSource.Lines)
		{
			var station = line.Stations.FirstOrDefault(x => x.Id == stationId);
			if (station == null)
				continue;
			return station;
		}
		return null;
	}

	public int GetRandomStationId()
	{
		var lineIndex = Core.NextRnd(_dataSource.Lines.Count);
		var line = _dataSource.Lines[lineIndex];
		return line.Stations[Core.NextRnd(line.Stations.Count)].Id;
	}

	public MetroLine GetLineByStationId(int stationId)
	{
		if (stationId <= 0)
		{
			throw new StationIdIsIncorrectException(nameof(stationId));
		}

		foreach (var line in _dataSource.Lines)
		{
			foreach (var station in line.Stations)
			{
				if (stationId == station.Id)
				{
					return line;
				}
			}
		}
		return null;
	}

	public ETrainDirection GetTrainDirection(int lineId, int startStationId, int finishStationId)
	{
		var line = GetLine(lineId);
		var startStation = GetStation(lineId, startStationId);
		var endStation = GetStation(lineId, finishStationId);
		return line.Stations.IndexOf(startStation) < line.Stations.IndexOf(endStation)
			? ETrainDirection.Forward
			: ETrainDirection.Backward;
	}

	public void StationLoadPassenger(Passenger passenger)
	{
		if (passenger == null)
		{
			throw new PassengerIsNullException(nameof(passenger));
		}

		if (passenger.Routes.Count == 0)
		{
			throw new Exception("У пассажира нет роутов");
		}

		var passengerRoute = passenger.Routes[0];
		var station = GetStation(passengerRoute.LineId, passengerRoute.StartStationId);
		if (passengerRoute.Direction == ETrainDirection.Forward)
		{
			station.ForwardPassengers.Add(passenger.Id, passenger);
		}
		else
		{
			station.BackwardPassengers.Add(passenger.Id, passenger);
		}
	}

	public void StationRemovePassenger(int stationId, Passenger passenger)
	{
		if (stationId <= 0)
		{
			throw new StationIdIsIncorrectException(nameof(stationId));
		}

		if (passenger == null)
		{
			throw new PassengerIsNullException(nameof(passenger));
		}

		if (passenger.Routes.Count == 0)
		{
			return;
		}

		var passengerRoute = passenger.Routes[0];
		var station = GetStation(passengerRoute.LineId, stationId);
		if (passengerRoute.Direction == ETrainDirection.Forward)
		{
			station.ForwardPassengers.Remove(passenger.Id);
		}
		else
		{
			station.BackwardPassengers.Remove(passenger.Id);
		}
	}

	public Dictionary<Guid, Passenger> GetPassengersToGo(int lineId, ETrainDirection direction, int trainStationId)
	{
		if (lineId <= 0)
		{
			throw new LineIdIsIncorrectException(nameof(lineId));
		}

		if (trainStationId <= 0)
		{
			throw new StationIdIsIncorrectException(nameof(trainStationId));
		}

		var station = GetStation(lineId, trainStationId);
		var passengers = direction == ETrainDirection.Forward ? station.ForwardPassengers : station.BackwardPassengers;
		return passengers.Where(x => x.Value.State == EPassengerState.Waits).ToDictionary();
	}

	public void UnloadPassengersFromTrain(Train train, int stationId)
	{
		if (stationId <= 0)
		{
			throw new StationIdIsIncorrectException(nameof(stationId));
		}

		if (train == null)
		{
			throw new TrainIsNullException(nameof(train));
		}

		//выгрузить из поезда
		foreach (var carriage in train.Carriages)
		{
			var listToRemove = new List<Guid>(500);
			foreach (var passenger in carriage.Passengers)
			{
				var route = passenger.Routes.FirstOrDefault(x => x.EndStationId == stationId);
				if (route == null)
				{
					continue;
				}

				//Пассажир должен будет выйти из поезда/вагона
				listToRemove.Add(passenger.Id);

				//Мы нашли роут с конечной текущей станцией
				//Надо удалить этот роут у пассажира
				passenger.Routes.Remove(route);

				//Если роутов нет, то пассажир удаляется вообще из метро
				if (passenger.Routes.Count == 0)
				{
					continue;
				}

				//Если пассажир едет куда-то дальше, поместить его на новую станцию
				route = passenger.Routes[0];
				var station = GetStation(route.StartStationId);
				if (route.Direction == ETrainDirection.Forward)
				{
					station.ForwardPassengers.TryAdd(passenger.Id, passenger);
				}
				else
				{
					station.BackwardPassengers.TryAdd(passenger.Id, passenger);
				}
			}

			//Удалить пассажира из коллекции вагона
			if (listToRemove.Count > 0)
			{
				carriage.Passengers.RemoveAll(x => listToRemove.Contains(x.Id));
			}
		}
	}

	public void AddTrain(int lineId, Train train)
	{
		if (train == null)
		{
			throw new TrainIsNullException(nameof(train));
		}

		if (lineId <= 0)
		{
			throw new LineIdIsIncorrectException(nameof(lineId));
		}

		GetLine(lineId).Trains.Add(train);
	}

    public void AddTickToPassengers()
    {
        foreach(var line in _dataSource.Lines)
		{
			foreach(var station in line.Stations)
			{
				AddTickToPassengers(station.ForwardPassengers);
                AddTickToPassengers(station.BackwardPassengers);
            }
		}
    }

    public int GetPassengerIntensity()
    {
	    return _dataSource.PassengerIntensityTimePeriods.GetCurrentCounter();
    }

    private void AddTickToPassengers(Dictionary<Guid, Passenger> passengers)
	{
        foreach (var passenger in passengers)
        {
            passenger.Value.TimePassedOnStation++;
            if (passenger.Value.TimePassedOnStation >= passenger.Value.TimeToStation)
            {
                passenger.Value.State = EPassengerState.Waits;
            }
        }
    }
}