using System.Windows;

namespace Metro.WPF.DrawHelpers;

/// <summary>
/// Базовый помощник для отрисовки элементов
/// </summary>
public abstract class BaseDrawHelper
{
	public readonly ResourceDictionary Resources;
	public const double Left = 10;
	public const double Top = 20;
	public const double MaxLineLength = 1100;
	public const int LineRectangleHeight = 210;

    protected BaseDrawHelper(ResourceDictionary resources)
	{
		Resources = resources;
	}
}