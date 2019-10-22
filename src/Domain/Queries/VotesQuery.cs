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
			if (!Guid.TryParse(content, out var questionId))
				yield break;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var vote in block.Votes.Where(i => i.QuestionId == questionId))
					yield return vote;
			}
		}
	}
}