using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Metro.Data.Contracts.Models.Lines;
using Metro.Services.Contracts.DataServices;

namespace Metro.WPF.DrawHelpers;

/// <summary>
/// Помощник для отрисовки линий метро
/// </summary>
internal class LineDrawHelper : BaseDrawHelper
{
	private readonly ITrainProcessService _trainProcessService;
	private readonly Dictionary<int, TextBlock> _lineCaptions = new();
	private readonly Dictionary<int, Border> _stationCaptions = new();

	public LineDrawHelper(ResourceDictionary resources, ITrainProcessService trainProcessService) : base(resources)
	{
		_trainProcessService = trainProcessService;
	}

	/// <summary>
	/// Отрисовать линию метро
	/// </summary>
	/// <param name="line">линия</param>
	/// <param name="index">индекс линии</param>
	/// <param name="canvas">область рисования</param>
	public void DrawLine(MetroLine line, int index, Canvas canvas)
	{
		var topForCaption = LineRectangleHeight * index;
		var topForLine = LineRectangleHeight * index + 40;
		var metrInPixel = line.Distance / MaxLineLength;//Сколько метров в пикселе
		var lineDistance = line.Distance / metrInPixel + Left;//Смещение небольшое

		canvas.Children.Add(CreateMetroLineRectangle(line.Id, index));
		var captionControl = GetMetroLineCaption(topForCaption, line.Name);
		_lineCaptions.Add(line.Id, captionControl);
		canvas.Children.Add(captionControl);
		canvas.Children.Add(GetLineBorder(line.Id, topForLine, Left, lineDistance));
		canvas.Children.Add(GetTrainButton(line.Id, topForCaption, MaxLineLength - Left));

		var totalMetrInPixel = Left;
		foreach (var station in line.Stations)
		{
			canvas.Children.Add(GetStationBorder(station.Name, topForLine, totalMetrInPixel, station.Id));
			totalMetrInPixel += station.Distance / metrInPixel;
		}
	}

	/// <summary>
	/// Обновить информацию о линии
	/// </summary>
	/// <param name="line">линия</param>
	/// <param name="index">индекс линии</param>
	public void UpdateLineInfo(MetroLine line, int index)
	{
		_lineCaptions[line.Id].Text = $"{line.Name} - Ⓜ️:{line.PassengersCount:D4} 🚇:{line.TrainPassengersCount:D4}";
		foreach (var station in line.Stations)
		{
			var s = station.BackwardPassengers.Count + station.ForwardPassengers.Count;
			((TextBlock)_stationCaptions[station.Id].Child).Text = $"[{s}] {station.Name}";
		}
	}

	/// <summary>
	/// Отрисовать границы (прямоугольник) для линии
	/// </summary>
	/// <param name="id">id линии</param>
	/// <param name="index">индекс линии</param>
	/// <returns></returns>
	private Rectangle CreateMetroLineRectangle(int id, int index)
	{
		var lineRectangle = new Rectangle
		{
			Name = $"metro_line_rectcangle_{id}",
		};
		Canvas.SetLeft(lineRectangle, 10);
		Canvas.SetTop(lineRectangle, LineRectangleHeight * index);

		return lineRectangle;
	}

	/// <summary>
	/// Создать элемент с названием линии
	/// </summary>
	/// <param name="y">отступ по y</param>
	/// <param name="text"></param>
	/// <returns></returns>
	private TextBlock GetMetroLineCaption(double y, string text)
	{
		var textBlock = new TextBlock
		{
			Text = text,
			Style = Resources["LabelTextMetro"] as Style
		};
		Canvas.SetLeft(textBlock, Left);
		Canvas.SetTop(textBlock, y);

		return textBlock;
	}

	/// <summary>
	/// Создать границы линии метро
	/// </summary>
	/// <param name="lineId">id линии</param>
	/// <param name="top">отступ сверху</param>
	/// <param name="left">отступ слева</param>
	/// <param name="distance">расстояние</param>
	/// <returns></returns>
	private Border GetLineBorder(int lineId, int top, double left, double distance)
	{
		return new Border
		{
			Child = new Line
			{
				Name = $"route_{lineId}",
				X1 = left + Left,
				Y1 = top,
				X2 = distance,
				Y2 = top,
			},
		};
	}

	/// <summary>
	/// Создать границы станции
	/// </summary>
	/// <param name="name">название станции</param>
	/// <param name="top">отступ сверху</param>
	/// <param name="left">отступ слева</param>
	/// <param name="id">id станции</param>
	/// <returns></returns>
	private Border GetStationBorder(string name, int top, double left, int id)
	{
		var stationElement = new Border
		{
			Child = new TextBlock
			{
				Text = name,
				Style = Resources["LabelTextBlock"] as Style,
				Name = $"txt_station_{id}"
			},
			Style = Resources["Label"] as Style
		};

		_stationCaptions.Add(id, stationElement);
		Canvas.SetTop(stationElement, top);
		Canvas.SetLeft(stationElement, left);

		return stationElement;
	}

	/// <summary>
	/// Создать кнопку создания поезда
	/// </summary>
	/// <param name="id">id линии</param>
	/// <param name="top">отступ сверху</param>
	/// <param name="left">отступ слева</param>
	/// <returns></returns>
	private Button GetTrainButton(int id, double top, double left)
	{
		var btn = new Button
		{
			Content = "Добавить поезд",
			Name = $"btn_{id}"
		};
		btn.Click += Btn_Click;

		Canvas.SetLeft(btn, left);
		Canvas.SetTop(btn, top);
		return btn;
	}

	/// <summary>
	/// Событие "создания поезда" по нажатию кнопки
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Btn_Click(object sender, RoutedEventArgs e)
	{
		var selectedLineId = int.Parse(((Button)sender).Name.Replace("btn_", string.Empty));
		_trainProcessService.StartNewTrain(selectedLineId);
	}
}