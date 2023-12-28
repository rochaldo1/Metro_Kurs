using Metro.Common;
using Metro.Data.Contracts.Models.Base;
using Metro.Data.Contracts.Models.Passengers;

namespace Metro.Data.Contracts.Models.Trains;

/// <summary>
/// Вагон
/// </summary>
public class Carriage : IdentityBase
{
    public Carriage(int id, bool isLast)
    {
        Id = id;
        IsLast = isLast;
        MaxPassengers = isLast ? Constants.LastCarriageMaxPassengersCount : Constants.AverageCarriageMaxPassengersCount;
        Passengers = new List<Passenger>(MaxPassengers);
    }

    /// <summary>
    /// Крайний вагон
    /// </summary>
    public bool IsLast { get; set; }

    /// <summary>
    /// Максимальное количество пассажиров в этом вагоне
    /// </summary>
    public int MaxPassengers { get; set; }

    /// <summary>
    /// Пассажиры, котоыре едут сейчас в вагоне
    /// </summary>
    public List<Passenger> Passengers { get; set; }
}