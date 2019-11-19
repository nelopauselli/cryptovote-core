using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Protocol
{
	public class SendCommunityCommand : ICommand
	{
		private readonly Community community;

		public SendCommunityCommand(Community community)
		{
			this.community = community;
		}

		public string Name => "Send Community";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(community);

			var header = new CommandHeader(CommandIds.SendCommunity, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}