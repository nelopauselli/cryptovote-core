using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Queries
{
	public class CommunitiesQueryCommand : ICommand<Community[]>
	{
		public string Name => "Communities query";

		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.QueryCommunities, 0);
			header.CopyTo(stream);
		}

		public Community[] Parse(Stream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			return Serializer.Parse<Community[]>(buffer);
		}
	}
}