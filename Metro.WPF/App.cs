using System.Windows;
using Metro.WPF.Views;

namespace Metro.WPF;

public class App : Application
{
	readonly MainWindow _mainWindow;

	// через систему внедрения зависимостей получаем объект главного окна
	public App(MainWindow mainWindow)
	{
		ShutdownMode = ShutdownMode.OnExplicitShutdown;
		_mainWindow = mainWindow;
	}
	protected override void OnStartup(StartupEventArgs e)
	{
		_mainWindow.Show();  // отображаем главное окно на экране
		base.OnStartup(e);
	}
}