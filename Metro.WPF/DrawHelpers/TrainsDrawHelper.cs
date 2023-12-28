using System.Text;
using Metro.Data.Contracts.Enums;
using Metro.Data.Contracts.Models.Lines;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Metro.Data.Contracts.Models.Trains;
using Metro.Common.Exceptions;

namespace Metro.WPF.DrawHelpers;

/// <summary>
/// Помощник для отрисовки поездов
/// </summary>
public class TrainsDrawHelper : BaseDrawHelper
{
	private readonly Dictionary<int, Ellipse> _trainEllipses = new();
	private readonly Dictionary<int, StackPanel> _trainPanels = new();
	private readonly ListBox _trainBox;
	private readonly Canvas _canvas;

	public TrainsDrawHelper(ResourceDictionary resources, Canvas canvas, ListBox trainBox) : base(resources)
	{
		_canvas = canvas;
		_trainBox = trainBox;
	}

	/// <summary>
	/// Очистить визуализацию от поездов
	/// </summary>
	public void Reset()
	{
		foreach (var panel in _trainPanels)
		{
			_trainBox.Items.Remove(panel.Value);
		}

		foreach (var e in _trainEllipses)
		{
			_canvas.Children.Remove(e.Value);
		}

		_trainEllipses.Clear();
		_trainPanels.Clear();
	}

	/// <summary>
	/// Движение поездов
	/// </summary>
	/// <param name="line">линия</param>
	/// <param name="metroLineIndex">индекс линии</param>
	public void MoveTrains(MetroLine line, int metroLineIndex)
	{
		var metrInPixel = line.Distance / MaxLineLength;//Сколько метров в пикселе

		foreach (var train in line.Trains)
		{
			if (!_trainEllipses.ContainsKey(train.Id))
			{
				CreateTrainElement(metroLineIndex, train.Id);
			}

			var e = _trainEllipses[train.Id];
			e.Style = GetTrainElementStyle(train.CurrentState.Status);
			UpdateTextTrain(train, line.Name[0]);

			//var left = (double)e.GetValue(Canvas.LeftProperty);
			double left;
			if (train.CurrentState.Direction == ETrainDirection.Forward)
				left = train.CurrentState.TotalDistance / metrInPixel + Left; //Смещение небольшое
			else
				left = MaxLineLength - train.CurrentState.TotalDistance / metrInPixel + Left;
			Canvas.SetLeft(e, left);
		}
	}

	/// <summary>
	/// Определение стиля для элемента поезда
	/// </summary>
	/// <param name="status"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	private Style GetTrainElementStyle(ETrainStatus status)
	{
		switch (status)
		{
			case ETrainStatus.Depot:
				return Resources["Depot"] as Style;
			case ETrainStatus.Speeding:
			case ETrainStatus.Rides:
			case ETrainStatus.Slowing:
				return Resources["Rides"] as Style;
			case ETrainStatus.Station:
				return Resources["Station"] as Style;
			default:
				throw new Exception("Нет подходящего стиля для статуса поезда");
		}
	}
		
	/// <summary>
	/// Обновление выводимой информации о поезде
	/// </summary>
	/// <param name="train">поезд</param>
	/// <param name="lineName">название линии</param>
	private void UpdateTextTrain(Train train, char lineName)
	{
        if (train is null)
        {
            throw new TrainIsNullException(nameof(train));
        }
        var stackPanel = _trainPanels[train.Id];
		var pgBar = stackPanel.Children.OfType<ProgressBar>().First();

		var pgBarValue = 0.0;
		if (train.PassengersCount > 0)
		{
			pgBarValue = train.PassengersCount * 1.0 / train.Carriages.Sum(x => x.MaxPassengers) * 100.0;
		}
		pgBar.Value = pgBarValue;

		var label = stackPanel.Children.OfType<TextBlock>().First();
		var sb = new StringBuilder($"{lineName}-🚇{train.Id}\t");
		sb.Append(GetRowTrain(train.CurrentState.Status, train.CurrentState.Direction));

		if (train.CurrentState.Status != ETrainStatus.Depot)
		{
			for (var i = 0; i < train.Carriages.Count; i++)
			{
				var carriage = train.Carriages[i];
				sb.Append($":[{i}:{carriage.Passengers.Count:D3}]:");
			}
			sb.Append($"\t[{train.PassengersCount}]");
		}

		label.Text = sb.ToString();
	}


    /*
	 * Стрелка влево: ← (Alt + 2190)
	 * Стрелка вправо: → (Alt + 2192)
	 * Стрелка вверх: ↑ (Alt + 2191)
	 * Стрелка вниз: ↓ (Alt + 2193)
	 * Источник: https://uchet-jkh.ru/i/kakie-simvoly-na-klaviature-ispolzuyutsya-v-kacestve-strelok
	 */

	/// <summary>
	/// Вывод символа состояния поезда
	/// </summary>
	/// <param name="state">статус поезда</param>
	/// <param name="direction">направление поезда</param>
	/// <returns></returns>
    private string GetRowTrain(ETrainStatus state, ETrainDirection direction)
	{
		var s = string.Empty;
		switch (state)
		{
			case ETrainStatus.Depot:
				s = "[В ДЕПО]";
				break;
			case ETrainStatus.Speeding:
				s = "[\u2191]";
				break;
			case ETrainStatus.Rides:
				s = direction == ETrainDirection.Forward ? "[\u2192]" : "[\u2190]";
				break;
			case ETrainStatus.Slowing:
				s = "[\u2193]";
				break;
			case ETrainStatus.Station:
				s = "[O]";
				break;
		}
		return $"{s}\t";
	}

	/// <summary>
	/// Создать элемент поезда
	/// </summary>
	/// <param name="index">индекс линии</param>
	/// <param name="trainId">id поезда</param>
	private void CreateTrainElement(int index, int trainId)
	{
        if (trainId <= 0)
        {
            throw new TrainIdIsIncorrectException(nameof(trainId));
        }

        var e = new Ellipse
		{
			Name = $"train_{trainId}",
			Style = Resources["Depot"] as Style,
		};
		var top = index * LineRectangleHeight + Top;
		Canvas.SetLeft(e, Left);
		Canvas.SetTop(e, top);
		_canvas.Children.Add(e);
		_trainEllipses.Add(trainId, e);
		CreateTrainPanel(trainId);
	}

	/// <summary>
	/// Создание панели для отображения информации о поезде
	/// </summary>
	/// <param name="trainId">id поезда</param>
	private void CreateTrainPanel(int trainId)
	{
		if(trainId <= 0)
		{
			throw new TrainIdIsIncorrectException(nameof(trainId));
		}

		var stackPanel = new StackPanel
		{
			Name = $"trainPanel_{trainId}"
		};

		var label = new TextBlock
		{
			Name = $"trainPanel_tbl_{trainId}"
		};

		var progressBar = new ProgressBar
		{
			Name = $"trainPgBar_{trainId}"
		};

		stackPanel.Margin = new Thickness(Left, Top, 0, 0);
		Grid.SetRow(stackPanel, 1);
		Grid.SetColumn(stackPanel, 1);
		stackPanel.Children.Add(progressBar);
		stackPanel.Children.Add(label);
		_trainPanels.Add(trainId, stackPanel);
		_trainBox.Items.Add(stackPanel);
	}
}