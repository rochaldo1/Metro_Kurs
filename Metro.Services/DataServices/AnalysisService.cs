using Metro.Common;
using Metro.Data.Contracts;
using Metro.Services.Contracts.DataServices;

namespace Metro.Services.DataServices;

internal class AnalysisService : IAnalysisService
{
	private readonly IDataSource _dataSource;

	private readonly List<double> _passengersWaitTimes = new();
	private readonly List<double> _workloadTrains = new();
	private readonly List<double> _workloadStations = new();

	public AnalysisService(IDataSource dataSource)
	{
		_dataSource = dataSource;
	}

	public void AddPassengerWaitTime(TimeSpan startWaitTime, TimeSpan endWaitTime)
	{
		if (!Core.IsWorkTime())
		{
			return;
		}

		var totalSeconds = endWaitTime.Subtract(startWaitTime).TotalSeconds;
		_passengersWaitTimes.Add(totalSeconds);
    }

	public List<double> GetAvgWaitTimePassengers()
	{
		return _passengersWaitTimes;
	}

	public List<double> GetWorkloadStations()
	{
		return _workloadStations;
	}

	public List<double> GetWorkloadTrains()
	{
		return _workloadTrains;
	}

	public void UpdateWorkload()
	{
		if (!Core.IsWorkTime())
		{
			return;
		}

		var stationStat = 0;
		var stationsCount = 0;
		var trainStat = 0;
		var trainCount = 0;
		foreach (var line in _dataSource.Lines)
		{
			foreach (var station in line.Stations)
			{
				stationsCount++;
				stationStat += station.BackwardPassengers.Count + station.ForwardPassengers.Count;
			}

			foreach (var train in line.Trains)
			{
				trainCount++;
				trainStat += train.PassengersCount;
			}
		}

		if (trainCount == 0 || stationsCount == 0)
		{
			return;
		}

		trainStat /= trainCount;
		stationStat /= stationsCount;

        _workloadStations.Add(stationStat);
		_workloadTrains.Add(trainStat);
    }
    public void Reset()
    {
        _passengersWaitTimes.Clear();
        _workloadTrains.Clear();
        _workloadStations.Clear();
    }
}