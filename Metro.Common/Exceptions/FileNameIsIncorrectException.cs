namespace Metro.Common.Exceptions;

public class FileNameIsIncorrectException : NullReferenceException
{
	private const string ExceptionMessage = "Имя файла не указано";

	public FileNameIsIncorrectException() : base(ExceptionMessage)
	{
	}
}