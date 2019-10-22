using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Scrutiny;

namespace Domain.Queries
{
	public class FiscalsQuery
	{
		private readonly Blockchain blockchain;

		public FiscalsQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IEnumerable<Fiscal> Execute(string content)
		{
			if (!Guid.TryParse(content, out var issueId))
				yield break;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var Fiscal in block.Fiscals.Where(i => i.IssueId == issueId))
					yield return Fiscal;
			}
		}
	}
}