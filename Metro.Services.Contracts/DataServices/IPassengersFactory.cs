namespace Metro.Services.Contracts.DataServices;

/// <summary>
/// Фабрика пассажиров
/// </summary>
public interface IPassengersFactory
{
    /// <summary>
    /// Запустить на станцию пассажиров
    /// Каждую секунду
    /// </summary>
    void GeneratePassengers();
}