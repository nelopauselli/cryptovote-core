using System;
using System.Linq;
using Domain.Scrutiny;

namespace Domain.Queries
{
	public class IssueQuery
	{
		private readonly Blockchain blockchain;

		public IssueQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public Issue Execute(string content)
		{
			var chunks = content.Split('#');

			if (chunks.Length != 2)
				return null;
			if (!Guid.TryParse(chunks[0], out var communityId))
				return null;
			if (!Guid.TryParse(chunks[1], out var issueId))
				return null;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var issue in block.Issues.Where(i => i.CommunityId == communityId && i.Id == issueId))
					return issue;
			}

			return null;
		}
	}
}