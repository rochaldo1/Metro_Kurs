using Metro.Common;
using Metro.Data.Contracts.Models;

namespace Metro.Services.Extensions;

internal static class TimePeriodExtensions
{
    /// <summary>
    /// Входит ли текущее время в период
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static bool InPeriod(this TimePeriodCounter obj)
    {
	    if (obj == null)
	    {
            throw new ArgumentNullException(nameof(obj), "Пеиод не задан");
	    }

        return Core.Time >= obj.FromTime && Core.Time <= obj.ToTime;
    }

    /// <summary>
    /// Получить период по текущему времени
    /// </summary>
    /// <param name="timePeriodCounters"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static int GetCurrentCounter(this IReadOnlyList<TimePeriodCounter> timePeriodCounters)
    {
	    if (timePeriodCounters == null)
	    {
		    throw new ArgumentNullException(nameof(timePeriodCounters), "Периоды не заданы");
	    }

		var waitTime = timePeriodCounters.FirstOrDefault(x => x.InPeriod());
        return waitTime?.Counter ?? 0;
    }
}