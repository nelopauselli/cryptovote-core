using System;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Scrutiny;

namespace Web.Controllers
{
	public class NodeAdapterIssues
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterIssues(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Issue[]> List(Guid communityId)
		{
			var command = new IssuesQueryMessage(communityId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task<Issue> Get(Guid communityId, Guid issueId)
		{
			var message = new IssueQueryMessage(communityId, issueId);
			return await nodeAdapter.GetResponse(message);
		}

		public async Task Add(Issue issue)
		{
			var command = new SendIssueMessage(issue).GetBytes();
			await nodeAdapter.GetResponse(command);
		}

		public async Task<IssueResult> Result(Guid communityId, Guid issueId)
		{
			var message = new IssueResultQueryMessage(communityId, issueId);
			return await nodeAdapter.GetResponse(message);
		}
	}
}