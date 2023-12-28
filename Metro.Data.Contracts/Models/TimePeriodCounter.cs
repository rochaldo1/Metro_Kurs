namespace Metro.Data.Contracts.Models;

/// <summary>
/// Нагрузка / ожидание поездов
/// </summary>
public class TimePeriodCounter
{
    /// <summary>
    /// Время ожидания на станции / Кол-во пассажиров
    /// </summary>
    public int Counter { get; set; }

    /// <summary>
    /// С какого преиода определяется ожидание
    /// </summary>
    public TimeSpan FromTime { get; set; }

    /// <summary>
    /// По какой период определяется ожидание
    /// </summary>
    public TimeSpan ToTime { get; set; }

    public TimePeriodCounter(int counter, TimeSpan fromTime, TimeSpan toTime)
    {
        Counter = counter;
        FromTime = fromTime;
        ToTime = toTime;
    }
}
