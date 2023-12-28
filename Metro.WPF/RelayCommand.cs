using System.Windows.Input;

namespace Metro.WPF;

/// <summary>
/// Класс для выполнения команд
/// </summary>
internal class RelayCommand : ICommand
{
	private readonly Action<object> _action;

	public RelayCommand(Action<object> action)
	{
		_action = action;
	}

	#region ICommand Members

	public bool CanExecute(object parameter)
	{
		return true;
	}

	public event EventHandler CanExecuteChanged;

	public void Execute(object parameter)
	{
		if (parameter != null)
		{
			_action(parameter);
		}
	}

	#endregion
}