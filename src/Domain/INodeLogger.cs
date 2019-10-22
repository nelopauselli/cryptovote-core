namespace Domain
{
	public interface INodeLogger
	{
		void Error(string message);
		void Warning(string message);
		void Information(string message);
		void Debug(string message);
	}
}