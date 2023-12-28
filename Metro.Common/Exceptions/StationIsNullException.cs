namespace Metro.Common.Exceptions;

public class StationIsNullException : ArgumentNullException
{
	private const string ExceptionMessage = "Станция не указана";

	public StationIsNullException(string paramName) : base(paramName, ExceptionMessage)
	{
	}
}