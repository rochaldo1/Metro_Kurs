using System.Windows;
using Metro.Data.Contracts;
using Metro.Services.Contracts.DataServices;
using Metro.WPF.ViewModels;

namespace Metro.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow
	(
		ILineService lineService,
		ITrainProcessService trainProcessService,
		IPassengersFactory passengersFactory,
		IDataSource dataSource,
		IAnalysisService analysisService
	)
	{
		InitializeComponent();
		DataContext = new ApplicationViewModel
		(
			Resources,
			canvas,
			trainBox,
			lineService,
			trainProcessService,
			passengersFactory,
			dataSource,
			analysisService
		);
		this.Closed += MainWindow_Closed;
	}

	private void MainWindow_Closed(object sender, EventArgs e)
	{
		Application.Current.Shutdown();
	}
}