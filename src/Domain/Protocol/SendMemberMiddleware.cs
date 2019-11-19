using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Protocol
{
	public class SendMemberMiddleware : IMiddleware
	{
		private readonly INode node;

		public SendMemberMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.SendMember) return false;

			var buffer = new byte[header.Length];
			peer.Read(buffer, 0, header.Length);

			var member = Serializer.Parse<Member>(buffer);

			node.Add(member);

			return true;
		}
	}
}