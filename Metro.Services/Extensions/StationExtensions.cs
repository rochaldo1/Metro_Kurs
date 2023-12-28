using Metro.Common;
using Metro.Common.Exceptions;
using Metro.Data.Contracts.Models.Lines;

namespace Metro.Services.Extensions;

internal static class StationExtensions
{
    /// <summary>
    /// Работает ли сейчас станция
    /// </summary>
    /// <param name="station"></param>
    /// <returns></returns>
    /// <exception cref="StationIsNullException"></exception>
    public static bool IsWork(this Station station)
    {
	    if (station == null)
	    {
		    throw new StationIsNullException(nameof(station));
	    }

        return
            Core.Time > station.Schedule.Start &&
            Core.Time < station.Schedule.End;
    }
}