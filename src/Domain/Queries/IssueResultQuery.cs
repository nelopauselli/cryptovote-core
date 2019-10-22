using System;
using Domain.Elections;

namespace Domain.Queries
{
	public class IssueResultQuery
	{
		private readonly Blockchain blockchain;

		public IssueResultQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IssueResult Execute(string content)
		{
			var chunks = content.Split('#');

			if (chunks.Length != 2)
				return null;
			if (!Guid.TryParse(chunks[0], out var communityId))
				return null;
			if (!Guid.TryParse(chunks[1], out var issueId))
				return null;

			return blockchain.GetResult(issueId);
		}
	}
}