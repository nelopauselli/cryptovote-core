using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Protocol
{
	public class GetLastBlockCommand : ICommand
	{
		public string Name => "Get last block";

		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.GetLastBlock, 0);
			header.CopyTo(stream);
		}
	}
}