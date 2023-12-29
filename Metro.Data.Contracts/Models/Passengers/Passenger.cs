using Metro.Data.Contracts.Enums;

namespace Metro.Data.Contracts.Models.Passengers;

/// <summary>
/// Пассажир
/// </summary>
public class Passenger
{
    public Passenger()
    {
        Routes = new List<PassengersRoute>();
        State = EPassengerState.Entered;
    }

    /// <summary>
    /// Время входа пассажира в метро
    /// </summary>
    public TimeSpan StartWaitTime { get; set; }
    /// <summary>
    /// Время посадки в поезд
    /// </summary>
    public TimeSpan EndWaitTime { get; set; }

    /// <summary>
    /// Id пассажира, guid потому что их будет очень много
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Маршрут пассажира
    /// </summary>
    public List<PassengersRoute> Routes { get; set; }
    /// <summary>
    /// Статус пассажира
    /// </summary>
    public EPassengerState State { get; set; }
    /// <summary>
    /// Сколько прождал на станции
    /// </summary>
    public int TimePassedOnStation { get; set; }
    /// <summary>
    /// Сколько идти до станции
    /// </summary>
    public int TimeToStation { get; set; }
}