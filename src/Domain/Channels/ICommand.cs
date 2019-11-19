using System.IO;

namespace Domain.Channels
{
	public interface ICommand<out T> : ICommand
	{
		T Parse(Stream stream, int length);
	}

	public interface ICommand
	{
		string Name { get; }
		void Send(Stream stream);
	}
}