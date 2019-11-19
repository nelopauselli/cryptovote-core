using System;
using System.Linq;
using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Queries
{
	public class CommunitiesQueryMiddleware : IMiddleware
	{
		private readonly INode node;

		public CommunitiesQueryMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryCommunities) return false;

			var queryCommunities = new CommunitiesQuery(node.Blockchain);
			var communities = queryCommunities.Execute();
			var buffer = Serializer.GetBytes(communities.ToArray());
			peer.Send(new QueryResponseCommand(buffer));

			return true;
		}
	}
}