namespace Metro.Services.Contracts.DataServices;

/// <summary>
/// Сервис анализа данных
/// </summary>
public interface IAnalysisService
{
	/// <summary>
	/// Добавление времени нахождения на станции
	/// </summary>
	/// <param name="startWaitTime">время начала ожидания</param>
	/// <param name="endWaitTime">время конца ожидания</param>
	void AddPassengerWaitTime(TimeSpan startWaitTime, TimeSpan endWaitTime);

	/// <summary>
	/// Получение данных времени ожидания
	/// </summary>
	/// <returns></returns>
	List<double> GetAvgWaitTimePassengers();

	/// <summary>
	/// Получение нагрузки станций
	/// </summary>
	/// <returns></returns>
	List<double> GetWorkloadStations();

	/// <summary>
	/// Получение нагрузки поездов
	/// </summary>
	/// <returns></returns>
	List<double> GetWorkloadTrains();

	/// <summary>
	/// Обновление статистики нагрузки
	/// </summary>
	void UpdateWorkload();

    /// <summary>
    /// Сбросить статистику
    /// </summary>
    void Reset();
}