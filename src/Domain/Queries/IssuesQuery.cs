using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Scrutiny;

namespace Domain.Queries
{
	public class IssuesQuery
	{
		private readonly Blockchain blockchain;

		public IssuesQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IEnumerable<Issue> Execute(string content)
		{
			if (!Guid.TryParse(content, out var communityId))
				yield break;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var issue in block.Issues.Where(i => i.CommunityId == communityId))
					yield return issue;
			}
		}
	}
}