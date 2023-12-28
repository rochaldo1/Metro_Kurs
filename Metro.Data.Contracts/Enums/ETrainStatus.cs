namespace Metro.Data.Contracts.Enums;

/// <summary>
/// Состояние поезда
/// </summary>
public enum ETrainStatus
{
	/// <summary>
	/// В депо
	/// </summary>
	Depot,

	/// <summary>
	/// Набирает скорость (начинает движение)
	/// </summary>
    Speeding,

	/// <summary>
	/// Едет
	/// </summary>
	Rides,

	/// <summary>
	/// Снижает скорость (заканчивает движение)
	/// </summary>
	Slowing,

	/// <summary>
	/// На станции
	/// </summary>
	Station,
}