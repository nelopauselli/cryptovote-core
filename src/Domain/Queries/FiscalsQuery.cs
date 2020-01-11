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

		[Obsolete]
		public IEnumerable<Fiscal> Execute(string content)
		{
			if (!Guid.TryParse(content, out var questionId))
				yield break;

			foreach (var block in blockchain.Trunk)
			{
				if (block?.Fiscals == null) continue;

				foreach (var fiscal in block.Fiscals.Where(i => i.QuestionId == questionId))
					yield return fiscal;
			}
		}

		public IEnumerable<Fiscal> Execute(Guid questionId)
		{
			foreach (var block in blockchain.Trunk)
			{
				if (block?.Fiscals == null) continue;

				foreach (var fiscal in block.Fiscals.Where(i => i.QuestionId == questionId))
					yield return fiscal;
			}
		}
	}
}