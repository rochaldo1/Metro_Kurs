using Metro.Data.Contracts.Enums;

namespace Metro.Data.Contracts.Models.Trains;

/// <summary>
/// Состояние поезда
/// </summary>
public class TrainState
{
	/// <summary>
	/// Статус поезда
	/// </summary>
	public ETrainStatus Status { set; get; }

	/// <summary>
	/// Куда сейчас движется поезд
	/// </summary>
	public ETrainDirection Direction { set; get; }

	/// <summary>
	/// Скорость, м/с
	/// </summary>
	public double Speed { set; get; }

	/// <summary>
	/// Дистанция до следующей станции
	/// </summary>
	public double DistanceToNextStation { set; get; }

	/// <summary>
	/// Станция к которой направляется
	/// </summary>
	public int NextStationId { set; get; }

	/// <summary>
	/// Станция к которой направляется
	/// </summary>
	public string NextStationName { set; get; }

	/// <summary>
	/// Станция, на которой находится поезд в текущий момент
	/// </summary>
	public int? CurrentStationId { set; get; }

	/// <summary>
	/// Пройденное расстояние до следующей станции
	/// </summary>
	public double Distance { set; get; }

	/// <summary>
	/// Общее пройденное расстояние по линии от начальной до конечной станции
	/// </summary>
	public double TotalDistance { set; get; }
}