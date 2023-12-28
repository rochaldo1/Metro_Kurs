using Metro.Common;
using Metro.Data.Contracts.Models.Base;
using Metro.Data.Contracts.Models.Passengers;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Metro.Data.Contracts.Models.Lines;

/// <summary>
/// Станция
/// </summary>
public class Station : NameBase
{
    /// <summary>
    /// Расписание станции
    /// </summary>
    public Schedule Schedule { set; get; }

    /// <summary>
    /// Длина участка до следующей станции
    /// </summary>
    public double Distance { get; set; }

    /// <summary>
    /// Id поезда, который едет на эту станцию
    /// </summary>
    public int? CurrentTrainId { set; get; }

    /// <summary>
    /// Пассажиры, которые едут вперед
    /// </summary>
    public Dictionary<Guid, Passenger> ForwardPassengers { get; set; } = new();

    /// <summary>
    /// Пассажиры, которые едут назад
    /// </summary>
    public Dictionary<Guid, Passenger> BackwardPassengers { get; set; } = new();

    /// <summary>
    /// Периоды стоянки
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<TimePeriodCounter> StationWaitTimes { get; } = new List<TimePeriodCounter>
    {
        new(60, new TimeSpan(6, 0, 0), new TimeSpan(0, 9, 59, 59, Constants.MaxMs)),
        new(80, new TimeSpan(10, 0, 0), new TimeSpan(0, 16, 59, 59, Constants.MaxMs)),
        new(60, new TimeSpan(17, 0, 0), new TimeSpan(0, 20, 59, 59, Constants.MaxMs)),
        new(120, new TimeSpan(21, 0, 0), new TimeSpan(0, 23, 59, 59, Constants.MaxMs)),
    };
}