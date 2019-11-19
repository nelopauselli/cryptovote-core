using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Protocol
{
	public class SendDocumentMiddleware : IMiddleware
	{
		private readonly INode node;

		public SendDocumentMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.SendDocument) return false;

			var buffer = new byte[header.Length];
			peer.Read(buffer, 0, header.Length);

			var member = Serializer.Parse<Document>(buffer);

			node.Add(member);

			return true;
		}
	}
}