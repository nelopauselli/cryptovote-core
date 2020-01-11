using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Elections;

namespace Domain.Queries
{
	public class UrnsQuery
	{
		private readonly Blockchain blockchain;

		public UrnsQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		[Obsolete]
		public IEnumerable<Urn> Execute(string content)
		{
			if (!Guid.TryParse(content, out var questionId))
				yield break;

			foreach (var block in blockchain.Trunk)
			{
				if (block?.Urns == null) continue;
				
				foreach (var urn in block.Urns.Where(i => i.QuestionId == questionId))
					yield return urn;
			}
		}

		public IEnumerable<Urn> Execute(Guid questionId)
		{
			foreach (var block in blockchain.Trunk)
			{
				if (block?.Urns == null) continue;

				foreach (var urn in block.Urns.Where(i => i.QuestionId == questionId))
					yield return urn;
			}
		}
	}
}