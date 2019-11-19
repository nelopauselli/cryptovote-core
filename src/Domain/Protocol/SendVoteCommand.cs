using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Protocol
{
	public class SendVoteCommand : ICommand
	{
		private readonly Vote vote;

		public SendVoteCommand(Vote vote)
		{
			this.vote = vote;
		}

		public string Name => "Send Vote";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(vote);

			var header = new CommandHeader(CommandIds.SendVote, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}