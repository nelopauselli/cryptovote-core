using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Protocol
{
	public class GetBlockMiddleware : IMiddleware
	{
		private readonly INode node;

		public GetBlockMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.GetBlock) return false;

			var hash = new byte[header.Length];
			peer.Read(hash, 0, header.Length);

			var block = node.Blockchain.GetBlock(hash);

			peer.Send(new SendBlockCommand(block));

			return true;
		}
	}
}