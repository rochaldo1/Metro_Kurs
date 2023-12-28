using Metro.Data.Contracts.Enums;

namespace Metro.Data.Contracts.Models.Passengers;

/// <summary>
/// Маршрут пассажира
/// </summary>
public class PassengersRoute
{
    public PassengersRoute(int startStationId, int endStationId, ETrainDirection direction, int lineId)
    {
        StartStationId = startStationId;
        EndStationId = endStationId;
        Direction = direction;
        LineId = lineId;
    }

    /// <summary>
    /// Линия, на которой пассажир поедет по этому маршруту
    /// </summary>
    public int LineId { get; set; }

    /// <summary>
    /// Направление марширута
    /// </summary>
    public ETrainDirection Direction { get; set; }

    /// <summary>
    /// Id начальной станции
    /// </summary>
    public int StartStationId { get; set; }

    /// <summary>
    /// Id конечной станции
    /// </summary>
    public int EndStationId { get; set; }
}