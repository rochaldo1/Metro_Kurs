using Metro.Common;

namespace Metro.Data.Contracts.Models;

/// <summary>
/// Расписание
/// </summary>
public class Schedule
{
	public Schedule()
	{
		Start = Constants.DefaultStartTime;
		End = Constants.DefaultEndTime;
	}

	public Schedule(TimeSpan start, TimeSpan end)
	{
		Start = start;
		End = end;
	}

	/// <summary>
	/// Время начала
	/// </summary>
	public TimeSpan Start { get; set; }

	/// <summary>
	/// Время окончания
	/// </summary>
	public TimeSpan End { get; set; }
}