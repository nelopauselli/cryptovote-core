using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Utils;

namespace Domain.Queries
{
	public class CommunityQueryMiddleware : IMiddleware
	{
		private readonly INode node;

		public CommunityQueryMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryCommunity) return false;

			var request = new byte[header.Length];
			peer.Read(request, 0, header.Length);

			var communityId = request.ToGuid();

			var communityQuery = new CommunityQuery(node.Blockchain);
			var community = communityQuery.Execute(communityId);
			var response = Serializer.GetBytes(community);
			peer.Send(new QueryResponseCommand(response));

			return true;
		}
	}
}