namespace Metro.Common;
/// <summary>
/// Ядро приложения
/// </summary>
public static class Core
{
	/// <summary>
	/// Начало работы метро
	/// </summary>
	private static readonly TimeSpan StartTime = TimeSpan.FromHours(Constants.StartTrainHour);

	/// <summary>
	/// Текущее время в метро
	/// </summary>
	public static TimeSpan Time { set; get; } = StartTime;

	/// <summary>
	/// Увеличить текущее время
	/// </summary>
	/// <param name="seconds"></param>
	public static void TimeIncrement(int seconds = 1)
	{
		Time += TimeSpan.FromSeconds(seconds);
		CheckDayTime();
	}

	/// <summary>
	/// Обнулить дни в текущем времени
	/// </summary>
	private static void CheckDayTime()
	{
		if (Time.TotalDays >= 1.0)
		{
			Time = Time.Add(TimeSpan.FromDays(-1));
		}
	}

	/// <summary>
	/// Работает ли сейчас метро
	/// </summary>
	/// <returns></returns>
	public static bool IsWorkTime()
	{
		if (Time >= TimeSpan.Zero && Time < StartTime)
			return false;
		return true;
	}

	/// <summary>
	/// Общий рандом
	/// </summary>
    private static readonly Random Random = new();

	/// <summary>
	/// Получить рандомное значение от нуля до end
	/// </summary>
	/// <param name="end"></param>
	/// <returns></returns>
	public static int NextRnd(int end) => Random.Next(0, end);

	/// <summary>
	/// Получить рандомное значение от start до end
	/// </summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <returns></returns>
    public static int NextRnd(int start, int end) => Random.Next(start, end);

    /// <summary>
    /// За сколько миллисекнд проходит одна секнуда
    /// </summary>
    public static int HoldTime = 1;
}
