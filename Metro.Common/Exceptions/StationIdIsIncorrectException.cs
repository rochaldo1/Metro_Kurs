namespace Metro.Common.Exceptions;

public class StationIdIsIncorrectException : ArgumentOutOfRangeException
{
	private const string ExceptionMessage = "Id станции не может быть <= 0";

	public StationIdIsIncorrectException(string paramName) : base(paramName, ExceptionMessage)
	{
	}
}