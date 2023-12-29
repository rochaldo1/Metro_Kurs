using Metro.Data.Contracts.Models.Trains;

namespace Metro.Services.Contracts.DataServices;

/// <summary>
/// Сервис для работы с поездами
/// </summary>
public interface ITrainProcessService
{
    /// <summary>
    /// Начать движение нового поезда
    /// </summary>
    /// <param name="lineId">id линии</param>
    /// <returns></returns>
    int StartNewTrain(int lineId);

    /// <summary>
    /// Загрузить процесс поезда
    /// </summary>
    /// <param name="train">поезд</param>
    /// <returns></returns>
    int LoadTrainProcess(Train train);

    /// <summary>
    /// Перезапуск процессов поездов
    /// </summary>
    void Reset();

    /// <summary>
    /// Сдвинуть поезд
    /// </summary>
    void Tick();
}