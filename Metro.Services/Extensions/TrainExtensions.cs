using Metro.Common;
using Metro.Common.Exceptions;
using Metro.Common.Helpers;
using Metro.Data.Contracts.Models.Trains;

namespace Metro.Services.Extensions;

internal static class TrainExtensions
{ 
	/// <summary>
	/// Найти свободный вагон для пассажира
	/// </summary>
	/// <param name="train">поезд</param>
	/// <returns></returns>
	/// <exception cref="TrainIsNullException"></exception>
    public static Carriage GetFreeCarriageForPassenger(this Train train)
    {
	    if (train == null)
	    {
		    throw new TrainIsNullException(nameof(train));
	    }

	    var carriages = train.Carriages.Where
	    (x =>
		    (!x.IsLast && x.Passengers.Count < Constants.AverageCarriageMaxPassengersCount) ||
		    (x.IsLast && x.Passengers.Count < Constants.LastCarriageMaxPassengersCount)
	    ).ToList();

        if (carriages.Count == 0)
            return null;

        var carriageIndex = Core.NextRnd(carriages.Count);
        return carriages[carriageIndex];
    }
	

    /// <summary>
    /// Создать вагоны в поезде
    /// </summary>
    /// <param name="train"></param>
    public static void CreateCarriages(this Train train)
	{
		if (train == null)
		{
			throw new TrainIsNullException(nameof(train));
		}

		for (var i = 0; i < Constants.DefaultCarriageCount; i++)
	    {
		    var carriage = new Carriage(IdGenerator.NextId, i is 0 or Constants.DefaultCarriageCount - 1);
		    train.Carriages.Add(carriage);
	    }
    }
}