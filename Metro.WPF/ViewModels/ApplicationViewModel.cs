using System.Windows;
using Metro.Common;
using Metro.Data.Contracts;
using Metro.Services.Contracts.DataServices;
using Metro.WPF.DrawHelpers;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using Metro.WPF.Views;
using Microsoft.Win32;
using System.Resources;

namespace Metro.WPF.ViewModels;

internal class ApplicationViewModel : INotifyPropertyChanged
{
	private const int MaxTick = 120;

	private readonly ILineService _lineService;
	private readonly IPassengersFactory _passengersFactory;
	private TrainsDrawHelper _trainsDrawHelper;
	private LineDrawHelper _lineDrawHelper;
	private readonly IDataSource _dataSource;
	private readonly ITrainProcessService _trainProcessService;
	private readonly IAnalysisService _analysisService;
	private readonly ResourceDictionary _resources;
	private readonly ListBox _trainBox;
	private DispatcherTimer _timer;

	/// <summary>
	/// Команда перезагрузки
	/// </summary>
	public ICommand ResetButtonCommand { get; set; }

	/// <summary>
	/// Команда сохранения
	/// </summary>
	public ICommand SaveButtonCommand { get; set; }

	/// <summary>
	/// Команда загрузки
	/// </summary>
	public ICommand LoadButtonCommand { get; set; }

	/// <summary>
	/// Команда вызова окна анализа
	/// </summary>
	public ICommand AnalysisWindowCallButton { get; set; }

	/// <summary>
	/// Компанда паузы
	/// </summary>
	public ICommand PauseButtonCommand { set; get; }

	/// <summary>
	/// Команда пропуска часа
	/// </summary>
	public ICommand FastForwardButtonCommand { set; get; }


	private int _tickCount;
	private string _title;

	/// <summary>
	/// Изменение названия окна
	/// </summary>
	public string Title
	{
		get => _title;
		set
		{
			_title = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// Скорость течения времени
	/// </summary>
	public int CurrentProgress
	{
		get => Core.HoldTime;
		set
		{
			Core.HoldTime = value;
			SetTimerSpeed();
		}
	}

	private readonly Canvas _canvas;

	public ApplicationViewModel
	(
		ResourceDictionary resources,
		Canvas canvas,
		ListBox trainBox,
		ILineService lineService,
		ITrainProcessService trainProcessService,
		IPassengersFactory passengersFactory,
		IDataSource dataSource, IAnalysisService analysisService)
	{
		_lineService = lineService;
		_passengersFactory = passengersFactory;
		_dataSource = dataSource;
		_analysisService = analysisService;
		_canvas = canvas;
		_trainProcessService = trainProcessService;
		_resources = resources;
		_trainBox = trainBox;

		//Загрузить данные состояния поездов
		dataSource.Lines.ForEach(line =>
		{
			line.Trains.ForEach(train => { trainProcessService.LoadTrainProcess(train); });
		});

		_lineDrawHelper = new LineDrawHelper(resources, trainProcessService);
		_trainsDrawHelper = new TrainsDrawHelper(resources, canvas, trainBox);

		DrawLines();
		StartTimer();

		ResetButtonCommand = new RelayCommand(ResetButtonCommandClick);
		SaveButtonCommand = new RelayCommand(SaveButtonCommandClick);
		AnalysisWindowCallButton = new RelayCommand(AnalysisWindowCallButtonClick);
		LoadButtonCommand = new RelayCommand(LoadButtonCommandClick);
		PauseButtonCommand = new RelayCommand(PauseButtonCommandClick);
		FastForwardButtonCommand = new RelayCommand(FastForwardButtonCommandClick);
	}

	/// <summary>
	/// Запуск времени
	/// </summary>
	private void StartTimer()
	{
		_timer = new DispatcherTimer();
		_timer.Tick += DispatcherTimer_Tick;
		SetTimerSpeed();
		_timer.Start();
	}

	/// <summary>
	/// Установить скорость течения времени
	/// </summary>
	private void SetTimerSpeed()
	{
		_timer.Interval = new TimeSpan(0, 0, 0, 0, Core.HoldTime);
	}

	/// <summary>
	/// Отрисовка линий
	/// </summary>
	private void DrawLines()
	{
		var lines = _lineService.GetLines();
		for (var metroLineIndex = 0; metroLineIndex < lines.Count; metroLineIndex++)
		{
			var line = lines[metroLineIndex];
			_lineDrawHelper.DrawLine(line, metroLineIndex, _canvas);
		}
	}

	/// <summary>
	/// Событие таймера
	/// </summary>
	/// <param name="sender">таймер</param>
	/// <param name="e"></param>
	private void DispatcherTimer_Tick(object sender, EventArgs e)
	{
		//Увеличить время
		Core.TimeIncrement();
		//Подвинуть поезда
		_trainProcessService.Tick();
		//Сгенерировать пассажиров
		_passengersFactory.GeneratePassengers();
		var lines = _lineService.GetLines();
		//Обновить данные по линии
		for (var metroLineIndex = 0; metroLineIndex < lines.Count; metroLineIndex++)
		{
			var line = lines[metroLineIndex];
			_lineDrawHelper.UpdateLineInfo(line, metroLineIndex);
			_trainsDrawHelper.MoveTrains(line, metroLineIndex);
		}

		//Изменить название окна
		Title = $"Время {Core.Time:g}";

		_tickCount++;

		_lineService.AddTickToPassengers();
		if (_tickCount == MaxTick)
		{
			_analysisService.UpdateWorkload();
			_tickCount = 0;
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (PropertyChanged != null)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	/// <summary>
	/// Сохранить состояние
	/// </summary>
	/// <param name="sender">кнопка</param>
	private void SaveButtonCommandClick(object sender)
	{
		while (true)
		{
			try
			{
				_dataSource.SaveLinesData();
				break;
			}
			catch (Exception e)
			{
				var messageBoxResult = MessageBox.Show($"Произошла ошибка, попробовать снова? {e.Message}", "Сохранение данных", MessageBoxButton.YesNo, MessageBoxImage.Error);
				if (messageBoxResult == MessageBoxResult.Yes)
					continue;
				break;
			}

		}
	}

	/// <summary>
	/// Загрузить состояние с файла
	/// </summary>
	/// <param name="sender">кнопка</param>
	private void LoadButtonCommandClick(object sender)
	{
		var ofd = new OpenFileDialog();
		if (ofd.ShowDialog() == true)
		{
			var fileName = ofd.FileName;
			_trainProcessService.Reset();
			_lineDrawHelper.Reset(_canvas);
			_trainsDrawHelper.Reset();
			_analysisService.Reset();
			_dataSource.LoadLinesData(fileName);

			_lineDrawHelper = new LineDrawHelper(_resources, _trainProcessService);
			_trainsDrawHelper = new TrainsDrawHelper(_resources, _canvas, _trainBox);

			DrawLines();
		}
	}

	/// <summary>
	/// Перезапуск всех процессов
	/// </summary>
	/// <param name="sender">кнопка</param>
	private void ResetButtonCommandClick(object sender)
	{
		var messageBoxResult = MessageBox.Show("Удалить все поезда и начать сначала?", "Перезагрузка состояния", MessageBoxButton.YesNo);
		if (messageBoxResult == MessageBoxResult.Yes)
		{
			_trainProcessService.Reset();
			_trainsDrawHelper.Reset();
			_analysisService.Reset();
		}
	}

	/// <summary>
	/// Вызов окна анализа
	/// </summary>
	/// <param name="sender">кнопка</param>
	private void AnalysisWindowCallButtonClick(object sender)
	{
		_timer.Stop();
		var analysisWindow = new AnalysisWindow(_analysisService);
		analysisWindow.ShowDialog();
		_timer.Start();
	}

	/// <summary>
	/// Пауза
	/// </summary>
	/// <param name="sender">кнопка</param>
	private void PauseButtonCommandClick(object sender)
	{
		if (!_timer.IsEnabled)
			_timer.Start();
		else
			_timer.Stop();
	}

	/// <summary>
	/// Пропустить час
	/// </summary>
	/// <param name="sender">кнопка</param>
	private void FastForwardButtonCommandClick(object sender)
	{
		//Добавить час
		Core.TimeIncrement(60 * 60);
	}
}