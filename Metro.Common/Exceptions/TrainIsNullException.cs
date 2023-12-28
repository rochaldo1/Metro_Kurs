namespace Metro.Common.Exceptions;

public class TrainIsNullException : ArgumentNullException
{
	private const string ExceptionMessage = "Поезд не может быть null";

	public TrainIsNullException(string paramName) : base(paramName, ExceptionMessage)
	{
	}
}