namespace Metro.Common.Exceptions;

public class LineIdIsIncorrectException : ArgumentOutOfRangeException
{
	private const string ExceptionMessage = "Id линии не может быть <= 0";

	public LineIdIsIncorrectException(string paramName) : base(paramName, ExceptionMessage)
	{
	}
}