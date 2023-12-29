using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using Metro.Services.Contracts.DataServices;
using ScottPlot;

namespace Metro.WPF.ViewModels;

/// <summary>
/// VM для окна анализа
/// </summary>
internal class AnalysisViewModel : INotifyPropertyChanged
{
	private readonly IAnalysisService _analysisService;

	private const string Folder = @"C:\Temp\Images\";

	public AnalysisViewModel(IAnalysisService analysisService)
	{
		_analysisService = analysisService;

		//Удаление всех предыдущих файлов
		foreach (var file in Directory.GetFiles(Folder))
		{
			try
			{
				File.Delete(file);
			}
			catch
			{
			}
		}
	}
	
	private int _selectedIndex;

	/// <summary>
	/// Выбранная диаграмма 
	/// </summary>
	public int SelectedIndex
	{
		get => _selectedIndex;
		set
		{
			if (_selectedIndex == value)
			{
				return;
			}
			_selectedIndex = value;
			DrawImageAndBind();
		}
	}

	/// <summary>
	/// Отрисовка диаграммы по выбранному индексу
	/// </summary>
	private void DrawImageAndBind()
	{
		switch (SelectedIndex)
		{
			case 1://Нагрузка на поезда
				DrawWorkload(_analysisService.GetWorkloadTrains());
				break;
			case 2://Нагрузка на станции
				DrawWorkload(_analysisService.GetWorkloadStations());
				break;
			case 3://Среднее время ожидания поезда
				AvgWaitTimePassenger();
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Генерация изображения гистограммы нагрузки
	/// </summary>
	/// <param name="workload">данные для гистограммы</param>
	private void DrawWorkload(List<double> workload)
	{ 
        var plt = new Plot(1000, 800);

		// create a histogram with a fixed number of bins
		var hist = ScottPlot.Statistics.Histogram.WithFixedBinSize(min: workload.Min(), max: workload.Max(), binSize: 10);

        // add random data to the histogram
        hist.AddRange(workload);

        // show the histogram counts as a bar plot
        double[] probabilities = hist.GetProbability();
        var bar = plt.AddBar(values: probabilities, positions: hist.Bins);
        bar.BarWidth = 5;

        // display vertical lines at points of interest
        var stats = new ScottPlot.Statistics.BasicStats(workload.ToArray());
        plt.AddVerticalLine(stats.Mean, Color.Black, 2, LineStyle.Solid, "Мат. ожидание");
        plt.AddVerticalLine(stats.Mean - stats.StDev, Color.Black, 2, LineStyle.Dash, "Среднекв. откл.");
        plt.AddVerticalLine(stats.Mean + stats.StDev, Color.Black, 2, LineStyle.Dash);
        plt.Legend(location: Alignment.UpperRight);

        // customize the plot style
        plt.Title("Нагрузка", size: 20);
		plt.YAxis.Label("Вероятность", size: 18);
		plt.XAxis.Label("Кол-во пассажиров", size: 18);
		plt.SetAxisLimits(yMin: 0);

		SavePlot(plt);
	}

    /// <summary>
    /// Генерация изображения гистограммы среднего времени ожидания
    /// </summary>
    private void AvgWaitTimePassenger()
	{
		var waitTimes = _analysisService.GetAvgWaitTimePassengers();

		var plt = new Plot(1000, 800);

		var hist = ScottPlot.Statistics.Histogram.WithFixedBinSize(min: waitTimes.Min(), max: waitTimes.Max(), binSize: 50);

		// add random data to the histogram
		hist.AddRange(waitTimes);

        // show the histogram counts as a bar plot
        double[] probabilities = hist.GetProbability();
        var bar = plt.AddBar(values: probabilities, positions: hist.Bins);
        bar.BarWidth = 10;

        // customize the plot style
        plt.Title("Среднее время ожидание пассажиров", size: 20);
		plt.YAxis.Label("Веротяность", size: 18);
		plt.XAxis.Label("Время (сек)", size: 18);
		plt.SetAxisLimits(yMin: 0);

		SavePlot(plt);
	}

	/// <summary>
	/// Сохранение изображения гистограммы
	/// </summary>
	/// <param name="plt"></param>
	private void SavePlot(Plot plt)
	{
		var fileName = $@"{Folder}stats_{Guid.NewGuid()}.png";
		plt.SaveFig(fileName);
		Image = new BitmapImage(new Uri(fileName, UriKind.Absolute));
	}

	private BitmapImage _image;

	/// <summary>
	/// Control-изображение для отображения
	/// </summary>
	public BitmapImage Image
	{
		get => _image;
		set
		{
			_image = value;
			OnPropertyChanged();
		}
	}
	
	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}