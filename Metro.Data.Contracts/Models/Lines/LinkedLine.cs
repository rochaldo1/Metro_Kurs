namespace Metro.Data.Contracts.Models.Lines;

/// <summary>
/// Связь линий
/// </summary>
public class LinkedLine
{
    /// <summary>
    /// Линия, на которую пассажиру нужно перейти
    /// </summary>
    public int DestinationLineId { get; set; }

    /// <summary>
    /// Id станции, с которой  пассажиру нужно перейти
    /// </summary>
    public int StationSourceId { get; set; }

    /// <summary>
    /// Id станции, на которую пассажиру нужно перейти
    /// </summary>
    public int StationDestinationId { get; set; }
}