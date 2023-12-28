namespace Metro.Common.Exceptions;

public class PassengerIsNullException : ArgumentNullException
{
	private const string ExceptionMessage = "Пассажир не может быть null";

	public PassengerIsNullException(string paramName) : base(paramName, ExceptionMessage)
	{
	}
}