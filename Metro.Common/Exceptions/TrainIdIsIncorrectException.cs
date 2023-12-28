namespace Metro.Common.Exceptions;

public class TrainIdIsIncorrectException : ArgumentOutOfRangeException
{
	private const string ExceptionMessage = "Id поезда не может быть <= 0";

	public TrainIdIsIncorrectException(string paramName) : base(paramName, ExceptionMessage)
	{
	}
}