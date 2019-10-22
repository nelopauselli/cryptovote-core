using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Scrutiny;
using Newtonsoft.Json;

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
			var command = new CommunitiesQueryMessage().GetBytes();
			var body = await nodeAdapter.GetResponse(command);
			return !string.IsNullOrWhiteSpace(body) ? JsonConvert.DeserializeObject<Community[]>(body) : null;
		}
		public async Task AddCommunity(Community community)
		{
			var command = new SendCommunityMessage(community);
			var data = command.GetBytes();
			await nodeAdapter.GetResponse(data);
		}

		public async Task<Community> Get(Guid communityId)
		{
			var command = new CommunityQueryMessage(communityId).GetBytes();
			var body = await nodeAdapter.GetResponse(command);
			return !string.IsNullOrWhiteSpace(body) ? JsonConvert.DeserializeObject<Community>(body) : null;
		}
	}
}