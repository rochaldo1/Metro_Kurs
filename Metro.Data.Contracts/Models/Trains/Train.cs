using Metro.Data.Contracts.Enums;
using Metro.Data.Contracts.Models.Base;

namespace Metro.Data.Contracts.Models.Trains;

/// <summary>
/// Поезд
/// </summary>
public class Train : IdentityBase
{
	/// <summary>
	/// Список вагонов в поезде
	/// </summary>
	public List<Carriage> Carriages { get; set; }

	/// <summary>
	/// Линия, на которой работает поезд
	/// </summary>
	public int LineId { get; set; }

	/// <summary>
	/// Текущее состояние поезда
	/// </summary>
	public TrainState CurrentState { set; get; }

	/// <summary>
	/// Количество пассажирво в поезде
	/// </summary>
	public int PassengersCount => Carriages.Sum(x => x.Passengers.Count);

	public Train(int id, int carriageCount, int lineId)
	{
		Id = id;
		LineId = lineId;
		CurrentState = new TrainState
		{
			Status = ETrainStatus.Depot,
			Direction = ETrainDirection.Forward,
		};

		Carriages = new List<Carriage>(carriageCount);
	}
}