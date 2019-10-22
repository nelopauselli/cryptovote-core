using System;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;

namespace Web.Controllers
{
	public class NodeAdapterVotes
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterVotes(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Vote[]> List(Guid questionId)
		{
			var command = new VotesQueryMessage(questionId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task Add(Vote vote)
		{
			var command = new SendVoteMessage(vote).GetBytes();
			await nodeAdapter.GetResponse(command);
		}
	}
}