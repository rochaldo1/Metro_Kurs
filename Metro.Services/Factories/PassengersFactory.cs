using Metro.Common;
using Metro.Data.Contracts.Models.Passengers;
using Metro.Services.Contracts.DataServices;
using Metro.Data.Contracts.Enums;
using Metro.Data.Contracts.Models.Lines;
using Metro.Common.Exceptions;

namespace Metro.Services.Factories;

internal class PassengersFactory : IPassengersFactory
{
    /// <summary>
    /// Периоды пассажиропотока
    /// </summary>
    private readonly ILineService _lineService;

    public PassengersFactory(ILineService lineService)
    {
        _lineService = lineService; 
    }

    public void GeneratePassengers()
    {
        //Сколько пассажиров запустить в метро
        var maxPass = _lineService.GetPassengerIntensity();
        var count = Core.NextRnd(maxPass);
        if (count <= 0)
            return;

        for (var i = 0; i < count; i++)
        {
            var passenger = CreatePassenger();
            //Растолкать пассажиров по станциям
            _lineService.StationLoadPassenger(passenger);
        }
    }

    /// <summary>
    /// Создать нового пассажира
    /// </summary>
    /// <returns></returns>
    private Passenger CreatePassenger()
    {
        while (true)
        {
            var startStationId = -1;
            var endStationId = -1;
            while (startStationId == endStationId)
            {
                //получаем рандомные станции, куда ехать пассажиру надо, пока они не будут отличаться
                startStationId = _lineService.GetRandomStationId();
                endStationId = _lineService.GetRandomStationId();
            }

            var routes = GetRoute(startStationId, endStationId, null);
            if (routes.Count == 0)
            {
                //Везем только тех, кто едет кудато а не просто пересаживается (таковы издержки random)
                continue;
            }

            return new Passenger 
            { 
                Routes = routes,
                StartWaitTime = Core.Time, 
                TimeToStation = Core.NextRnd(Constants.PassengerMinTimeToStation, Constants.PassengerMaxTimeToStation)
            };
        }
    }
    
    /// <summary>
    /// Проверка, что пассажир не выходит на станции пересадки
    /// </summary>
    /// <param name="line"></param>
    /// <param name="startStationId"></param>
    /// <param name="endStationId"></param>
    /// <returns></returns>
    private bool CheckStationsLink(MetroLine line, int startStationId, int endStationId)
	{
		foreach (var linkedLine in line.LinkedLines)
		{
			if 
			(
				(linkedLine.StationSourceId == startStationId || linkedLine.StationDestinationId == startStationId) &&
				(linkedLine.StationSourceId == endStationId || linkedLine.StationDestinationId == endStationId)
			)
			{
				return false;
			}
		}

        return true;
	}

    /// <summary>
    /// Получение маршрутов
    /// </summary>
    /// <param name="startStationId">id начальной станции</param>
    /// <param name="endStationId">id конечной станции</param>
    /// <param name="currentLinkedId">текущий id пересадки</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
	private List<PassengersRoute> GetRoute(int startStationId, int endStationId, string currentLinkedId)
    {
	    if (startStationId <= 0)
	    {
		    throw new StationIdIsIncorrectException(nameof(startStationId));
	    }

	    if (endStationId <= 0)
	    {
		    throw new StationIdIsIncorrectException(nameof(endStationId));
	    }

		var routes = new List<PassengersRoute>();
        
        if (startStationId == endStationId)
        {
	        return routes;
        }
        
        var lineStart = _lineService.GetLineByStationId(startStationId);
        var lineEnd = _lineService.GetLineByStationId(endStationId);

        if (!CheckStationsLink(lineStart, startStationId, endStationId) || !CheckStationsLink(lineEnd, startStationId, endStationId))
        {
	        return routes;
        }

		if (lineStart.Id == lineEnd.Id)
        {
            var direction = _lineService.GetTrainDirection(lineStart.Id, startStationId, endStationId);
            var route = new PassengersRoute(startStationId, endStationId, direction, lineStart.Id);
            if (route.StartStationId == route.EndStationId)
            {
	            return routes;
            }
			routes.Add(route);
            return routes;
        }

        foreach (var link in lineStart.LinkedLines)
        {
	        if (link.StationDestinationId == endStationId)
	        {
                continue;
	        }

	        if (currentLinkedId == link.Code)
	        {
		        continue;
	        }

            var innerRoutes = GetRoute(link.StationDestinationId, endStationId, link.Code);
            if (innerRoutes.Count == 0)
            {
                continue;
            }
            var flag = false;
            if (innerRoutes.Count == 1)
            {
                var r = innerRoutes[0];
                if (r.StartStationId == r.EndStationId)
                {
                    flag = true;
                }
            }
            var direction = _lineService.GetTrainDirection(lineStart.Id, startStationId, link.StationSourceId);
            var route = new PassengersRoute(startStationId, link.StationSourceId, direction, lineStart.Id);
            if (route.StartStationId == route.EndStationId)
            {
	            if (!flag)
	            {
		            routes.AddRange(innerRoutes);
                    break;
	            }
            }

            routes.Add(route);
            if (!flag)
            {
                routes.AddRange(innerRoutes);
            }
            break;
        }

        if (routes.Count == 0)
        {
            routes = GetRoute(endStationId, startStationId, null);
            routes.Reverse();
            foreach (var route in routes)
            {
                (route.StartStationId, route.EndStationId) = (route.EndStationId, route.StartStationId);
                route.Direction = route.Direction == ETrainDirection.Backward ? ETrainDirection.Forward : ETrainDirection.Backward;
            }
        }

        return routes;
    }
}