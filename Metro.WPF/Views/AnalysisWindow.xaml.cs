using System.Windows;
using Metro.Services.Contracts.DataServices;
using Metro.WPF.ViewModels;

namespace Metro.WPF.Views;

/// <summary>
/// Логика взаимодействия для AnalysisWindow.xaml
/// </summary>
public partial class AnalysisWindow : Window
{
	public AnalysisWindow(IAnalysisService analysisService)
	{
		InitializeComponent();

		DataContext = new AnalysisViewModel
		(
			analysisService
		);

	}
}