using Metro.Data.Contracts.Models;
using Metro.Data.Contracts.Models.Lines;

namespace Metro.Data.Contracts;

public interface IDataSource
{
    /// <summary>
    /// Список линий метро
    /// </summary>
    List<MetroLine> Lines { get; }

    IReadOnlyList<TimePeriodCounter> PassengerIntensityTimePeriods { get; }

	/// <summary>
	/// Загрузить линии метро из файла
	/// </summary>
	void LoadLinesData();

    /// <summary>
    /// Сохранить состояние линий
    /// </summary>
    void SaveLinesData();

    /// <summary>
    /// Перезапуск линий
    /// </summary>
    void Reset();

    /// <summary>
    /// Загрузить линии метро из заданного файла
    /// </summary>
    /// <param name="fileName"></param>
    void LoadLinesData(string fileName);
}
