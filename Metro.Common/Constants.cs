namespace Metro.Common;

public static class Constants
{
    public static readonly TimeSpan DefaultStartTime = new(6, 0, 0);
    public static readonly TimeSpan DefaultEndTime = new(0, 23, 59, 59, MaxMs);

    /// <summary>
    /// Максимальная скорость движения поезда м/с
    /// </summary>
    public const double MaxTrainSpeed = 16.6667;
    
    /// <summary>
    /// Количество вагонов по умолчанию
    /// </summary>
    public const int DefaultCarriageCount = 5;

    /// <summary>
    /// Ускорение при наборе скорости (м/с)
    /// </summary>
    public const double AccelerationSpeedUp = 1.3;

    /// <summary>
    /// Ускорение при снижении скорости (м/с)
    /// </summary>
    public const double AccelerationSlowDown = 1.1;

    /// <summary>
    /// Вместимость вагона
    /// </summary>
    public const int AverageCarriageMaxPassengersCount = 330;

    /// <summary>
    /// Вместимость крайних вагонов
    /// </summary>
    public const int LastCarriageMaxPassengersCount = 300;

    /// <summary>
    /// Миллисекунды - конец секунды
    /// </summary>
    public const int MaxMs = 999;

    /// <summary>
    /// Сколько секунд обычно поезд ждет на станции после того как приедет из депо на первую станцию
    /// </summary>
    public const int PauseOnStationAfterDepot = 60;

    /// <summary>
    /// Время старта поездов
    /// </summary>
    public const double StartTrainHour = 5.75;

    /// <summary>
    /// Минимум времени, чтобы дойти до станции
    /// </summary>
    public const int PassengerMinTimeToStation = 30;

    /// <summary>
    /// Максимум времени, чтобы дойти до станции
    /// </summary>
    public const int PassengerMaxTimeToStation = 120;
}