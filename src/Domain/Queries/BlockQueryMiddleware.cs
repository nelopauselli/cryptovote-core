using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Queries
{
	public class BlockQueryMiddleware : IMiddleware
	{
		private readonly INode node;

		public BlockQueryMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryBlock) return false;

			var hash = new byte[header.Length];
			peer.Read(hash, 0, header.Length);

			var block = node.Blockchain.GetBlock(hash);
			var buffer = Serializer.GetBytes(block);

			peer.Send(new QueryResponseCommand(buffer));

			return true;
		}
	}
}