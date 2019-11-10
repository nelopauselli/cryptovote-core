using System.IO;

namespace Domain.Channels
{
	public interface IChannel
	{
		void TalkWithClient(Stream stream);
	}
}