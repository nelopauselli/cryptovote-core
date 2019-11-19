using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;

namespace Web.Controllers
{
	public class NodeAdapterCommunities
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterCommunities(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<IEnumerable<Community>> ListCommunities()
		{
			var command = new CommunitiesQueryCommand();
			var response = await nodeAdapter.GetResponse(command);
			return response;
		}
		public async Task AddCommunity(Community community)
		{
			var command = new SendCommunityCommand(community);
			await nodeAdapter.Send(command);
		}

		public async Task<Community> Get(Guid communityId)
		{
			var command = new CommunityQueryCommand(communityId);
			var response = await nodeAdapter.GetResponse(command);
			return response;
		}
	}
}