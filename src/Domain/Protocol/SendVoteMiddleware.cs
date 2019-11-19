using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Protocol
{
	public class SendVoteMiddleware:IMiddleware{
		private readonly INode node;

		public SendVoteMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.SendVote) return false;

			var buffer = new byte[header.Length];
			peer.Read(buffer, 0, header.Length);

			var vote = Serializer.Parse<Vote>(buffer);

			node.Add(vote);

			return true;
		}
	}
}