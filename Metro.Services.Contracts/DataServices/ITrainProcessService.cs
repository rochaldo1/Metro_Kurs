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
    /// На участке до следующей станции есть поезд на расстоянии тормозного пути
    /// </summary>
    /// <param name="trainId">текщуий поезд</param>
    /// <param name="stationId">станция на которую едем</param>
    /// <param name="breakingDistance">тормозной путь - безопасная дистанция</param>
    /// <returns></returns>
    bool HasDangerDistance(int trainId, int stationId, double breakingDistance);

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