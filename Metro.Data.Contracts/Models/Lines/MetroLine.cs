using Metro.Data.Contracts.Models.Base;
using Metro.Data.Contracts.Models.Trains;

namespace Metro.Data.Contracts.Models.Lines;

/// <summary>
/// Линия метро
/// </summary>
public class MetroLine : NameBase
{ 
	/// <summary>
	/// Общая дистанция линии
	/// </summary>
	public double Distance { set; get; }

	/// <summary>
	/// Поезда
	/// </summary>
	public List<Train> Trains { set; get; }

	/// <summary>
	/// Список станций
	/// </summary>
	public MetroLineStations Stations { set; get; }

	/// <summary>
	/// Список пересадочных узлов
	/// </summary>
	public List<LinkedLine> LinkedLines { set; get; }

	/// <summary>
	/// Количество пассажиров на станциях
	/// </summary>
	public int PassengersCount => Stations.Sum(x => x.ForwardPassengers.Count + x.BackwardPassengers.Count);

    /// <summary>
    /// Количество пассажиров в поездах станциях
    /// </summary>
    public int TrainPassengersCount => Trains.Sum(x => x.PassengersCount);
}