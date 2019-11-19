using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Queries
{
	public class LastBlockQueryMiddleware : IMiddleware
	{
		private readonly INode node;

		public LastBlockQueryMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryLastBlock) return false;

			var block = node.Blockchain.Last;
			var buffer = Serializer.GetBytes(block);

			peer.Send(new QueryResponseCommand(buffer));

			return true;
		}
	}
}