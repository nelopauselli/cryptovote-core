using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Protocol
{
	public class SendBlockMiddleware : IMiddleware
	{
		private readonly INode node;

		public SendBlockMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.SendBlock) return false;

			var buffer = new byte[header.Length];
			peer.Read(buffer, 0, header.Length);

			var member = Serializer.Parse<Block>(buffer);

			node.Add(member);

			return true;
		}
	}
}