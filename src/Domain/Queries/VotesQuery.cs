using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Elections;

namespace Domain.Queries
{
	public class VotesQuery
	{
		private readonly Blockchain blockchain;

		public VotesQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IEnumerable<Vote> Execute(string content)
		{
			if (!Guid.TryParse(content, out var issueId))
				yield break;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var vote in block.Votes.Where(i => i.IssueId == issueId))
					yield return vote;
			}
		}
	}
}