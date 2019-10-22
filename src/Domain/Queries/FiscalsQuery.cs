using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Elections;

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
			if (!Guid.TryParse(content, out var questionId))
				yield break;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var Fiscal in block.Fiscals.Where(i => i.QuestionId == questionId))
					yield return Fiscal;
			}
		}
	}
}