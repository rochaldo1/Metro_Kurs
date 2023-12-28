using Metro.Common;
using Metro.Common.Exceptions;
using Metro.Common.Helpers;
using Metro.Data.Contracts;
using Metro.Data.Contracts.Models;
using Metro.Data.Contracts.Models.Lines;
using Metro.Data.Models;
using Newtonsoft.Json;

namespace Metro.Data;

internal class DataSource : IDataSource
{
	private const string Folder = @"C:\Temp\";
	private const string DataSourceFileName = $"{Folder}dataSource.txt";
	private const string SystemDataSourceFileName = $"{Folder}systemDataSource.txt";
	private const string PassengerIntensityDataSourceFileName = $"{Folder}passengerIntensityDataSource.txt";

	public List<MetroLine> Lines { private set; get; } = new();
	public IReadOnlyList<TimePeriodCounter> PassengerIntensityTimePeriods { private set; get; }
	
	public void LoadLinesData()
	{
		//Основные данные по линиям
		var txtData = File.ReadAllText(DataSourceFileName);
		Lines = JsonConvert.DeserializeObject<List<MetroLine>>(txtData);

		//Системные данные
		if (File.Exists(SystemDataSourceFileName))
		{
			txtData = File.ReadAllText(SystemDataSourceFileName);
			var systemData = JsonConvert.DeserializeObject<SystemData>(txtData);
			Core.Time = systemData.CurrentTime;
			IdGenerator.SetLastId(systemData.LastCoreId);
		}

		LoadPassengerIntensityDataSource();
	}

	public void LoadLinesData(string fileName)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			throw new FileNameIsIncorrectException();
		}

		Reset();
		var txtData = File.ReadAllText(fileName);
		Lines = JsonConvert.DeserializeObject<List<MetroLine>>(txtData);

		LoadPassengerIntensityDataSource();
	}

	public void SaveLinesData()
	{
		var textData = JsonConvert.SerializeObject(Lines, Formatting.Indented);
		File.WriteAllText(DataSourceFileName, textData);

		var systemData = new SystemData
		{
			CurrentTime = Core.Time,
			LastCoreId = IdGenerator.CurrentId,
		};

		textData = JsonConvert.SerializeObject(systemData, Formatting.Indented);
		File.WriteAllText(SystemDataSourceFileName, textData);

		textData = JsonConvert.SerializeObject(PassengerIntensityTimePeriods, Formatting.Indented);
		File.WriteAllText(PassengerIntensityDataSourceFileName, textData);
	}

	public void Reset()
	{
		//Начальное время сбросить
		Core.Time = TimeSpan.FromHours(Constants.StartTrainHour);
		foreach (var line in Lines)
		{
			foreach (var station in line.Stations)
			{
				//Сбросить станции
				station.BackwardPassengers.Clear();
				station.ForwardPassengers.Clear();
				station.CurrentTrainId = null;
			}
			//Очистить поезда
			line.Trains.Clear();
		}
	}

	/// <summary>
	/// Загрузить интенсивность пассажиров из файла или задать дефолтные значения
	/// </summary>
	private void LoadPassengerIntensityDataSource()
	{
		if (File.Exists(PassengerIntensityDataSourceFileName))
		{
			var txtData = File.ReadAllText(PassengerIntensityDataSourceFileName);
			PassengerIntensityTimePeriods = JsonConvert.DeserializeObject<List<TimePeriodCounter>>(txtData);
		}
		else
		{
			PassengerIntensityTimePeriods = GetDefaultPassengerIntensityData();
		}
	}

	/// <summary>
	/// Получить дефолтную нагрузку пассажиров на метро по периодам времени
	/// </summary>
	/// <returns></returns>
	private List<TimePeriodCounter> GetDefaultPassengerIntensityData()
	{
		const int passengerBySeconds = 5;
		return new List<TimePeriodCounter>
		{
			new(0, new TimeSpan(0), new TimeSpan(0, 5, 59, 59, Constants.MaxMs)),
			new(passengerBySeconds - 2, new TimeSpan(6, 0, 0), new TimeSpan(0, 6, 59, 59, Constants.MaxMs)),
			new(passengerBySeconds - 1, new TimeSpan(7, 0, 0), new TimeSpan(0, 7, 59, 59, Constants.MaxMs)),
			new(passengerBySeconds + 1, new TimeSpan(8, 0, 0), new TimeSpan(0, 8, 15, 59, Constants.MaxMs)),
			new(passengerBySeconds * 2, new TimeSpan(8, 16, 0), new TimeSpan(0, 9, 59, 59, Constants.MaxMs)),
			new(passengerBySeconds + 1, new TimeSpan(10, 0, 0), new TimeSpan(0, 12, 59, 59, Constants.MaxMs)),
			new(passengerBySeconds * 2, new TimeSpan(13, 0, 0), new TimeSpan(0, 17, 59, 59, Constants.MaxMs)),
			new(passengerBySeconds + 3, new TimeSpan(18, 0, 0), new TimeSpan(0, 19, 59, 59, Constants.MaxMs)),
			new(passengerBySeconds, new TimeSpan(20, 0, 0), new TimeSpan(0, 21, 59, 59, Constants.MaxMs)),
			new(passengerBySeconds - 1, new TimeSpan(22, 0, 0), new TimeSpan(0, 22, 59, 59, Constants.MaxMs)),
			new(passengerBySeconds - 2, new TimeSpan(23, 0, 0), new TimeSpan(0, 23, 35, 59, Constants.MaxMs)),
			new(2, new TimeSpan(23, 36, 0), new TimeSpan(0, 23, 59, 59, Constants.MaxMs))
		};
	}
}