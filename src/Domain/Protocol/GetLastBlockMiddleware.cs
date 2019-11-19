using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Protocol
{
	public class GetLastBlockMiddleware : IMiddleware
	{
		private readonly INode node;

		public GetLastBlockMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.GetLastBlock) return false;

			var block = node.Blockchain.Last;
			
			peer.Send(new SendBlockCommand(block));

			return true;
		}
	}
}