namespace Metro.Data.Contracts.Models.Lines;

/// <summary>
/// Список станций метро
/// </summary>
public class MetroLineStations : List<Station>
{
    /// <summary>
    /// Получить следущую станцию метро
    /// </summary>
    /// <param name="currentStationId">Станция, на которой сейчас находится поезд</param>
    /// <returns>Null - значит мы доехали до конечной</returns>
    public Station GetNextStation(int currentStationId)
    {
        var currentStation = this.First(x => x.Id == currentStationId);
        var currentStationIndex = IndexOf(currentStation);
        var i = currentStationIndex + 1;
        return Count > i ? this[i] : null;
    }

    /// <summary>
    /// Получить предыдущую станцию метро
    /// </summary>
    /// <param name="currentStationId">Станция, на которой сейчас находится поезд</param>
    /// <returns>Null - значит мы доехали до конечной</returns>
    public Station GetPrevStation(int currentStationId)
    {
        var currentStation = this.First(x => x.Id == currentStationId);
        var currentStationIndex = IndexOf(currentStation);
        var i = currentStationIndex - 1;
        return i >= 0 ? this[i] : null;
    }
}