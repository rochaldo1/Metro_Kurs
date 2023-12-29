using Metro.Data.Contracts.Enums;
using Metro.Data.Contracts.Models.Lines;
using Metro.Data.Contracts.Models.Passengers;
using Metro.Data.Contracts.Models.Trains;

namespace Metro.Services.Contracts.DataServices;

/// <summary>
/// Сервис для работы с линиями
/// </summary>
public interface ILineService
{
    /// <summary>
    /// Получить линию по id
    /// </summary>
    /// <param name="id">id линии</param>
    /// <returns></returns>
    MetroLine GetLine(int id);

    /// <summary>
    /// Получить список линий
    /// </summary>
    /// <returns></returns>
    List<MetroLine> GetLines();

    /// <summary>
    /// Получить станцию метро по Id линии метро и станции метро
    /// </summary>
    /// <param name="lineId">id линии</param>
    /// <param name="stationId">id станции</param>
    /// <returns>Станцию метро</returns>
    Station GetStation(int lineId, int stationId);

    /// <summary>
    /// Получть Id случайной станции на случайной линии
    /// </summary>
    /// <returns>Id случайной станции на случайной линии </returns>
    int GetRandomStationId();

    /// <summary>
    /// Получить линию по id станции
    /// </summary>
    /// <param name="stationId">id станции</param>
    /// <returns></returns>
    MetroLine GetLineByStationId(int stationId);

    /// <summary>
    /// Получить направление поезда
    /// </summary>
    /// <param name="lineId">id линии</param>
    /// <param name="startStationId">id начальной станции</param>
    /// <param name="finishStationId">id конечной станции</param>
    /// <returns></returns>
    ETrainDirection GetTrainDirection(int lineId, int startStationId, int finishStationId);

    /// <summary>
    /// Добавление пассажира в станцию
    /// </summary>
    /// <param name="passenger">пассажир</param>
    void StationLoadPassenger(Passenger passenger);

    /// <summary>
    /// Удаление пассажира со станции
    /// </summary>
    /// <param name="stationId">id станции</param>
    /// <param name="passenger">пассажир</param>
    void StationRemovePassenger(int stationId, Passenger passenger);

    /// <summary>
    /// Получить список пассажиров, которые могут поехать на поезде
    /// </summary>
    /// <param name="lineId"> Id линии, по которой движется поезд</param>
    /// <param name="direction"> Направление поезда</param>
    /// <param name="trainStationId"> Текущая станция</param>
    /// <returns> Список пассажиров, которые могут поехать на поезде</returns>
    Dictionary<Guid, Passenger> GetPassengersToGo(int lineId, ETrainDirection direction, int trainStationId);

    /// <summary>
    /// Выгрузить пассажиров из поезда
    /// </summary>
    /// <param name="train">поезд</param>
    /// <param name="stationId">id станции</param>
    void UnloadPassengersFromTrain(Train train, int stationId);

    /// <summary>
    /// Добавить поезд на линию
    /// </summary>
    /// <param name="lineId">id линии</param>
    /// <param name="train">поезд</param>
    void AddTrain(int lineId, Train train);

    /// <summary>
    /// Добавить секунду ко времени ожидания поезда
    /// </summary>
    void AddTickToPassengers();

    /// <summary>
    /// Получить интенсивность пассажиров
    /// </summary>
    /// <returns></returns>
    int GetPassengerIntensity();
}