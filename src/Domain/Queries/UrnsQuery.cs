using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Scrutiny;

namespace Domain.Queries
{
	public class UrnsQuery
	{
		private readonly Blockchain blockchain;

		public UrnsQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IEnumerable<Urn> Execute(string content)
		{
			if (!Guid.TryParse(content, out var issueId))
				yield break;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var urn in block.Urns.Where(i => i.IssueId == issueId))
					yield return urn;
			}
		}
	}
}