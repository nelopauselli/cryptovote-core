using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Queries
{
	public class QueryResponseCommand : ICommand
	{
		private readonly byte[] buffer;

		public QueryResponseCommand(byte[] buffer)
		{
			this.buffer = buffer;
		}

		public string Name => "Response Query";
		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.QueryResponse, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}