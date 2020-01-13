using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Elections;

namespace Domain.Queries
{
	public class VoteQuery
	{
		private readonly Blockchain blockchain;

		public VoteQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IEnumerable<Vote> Execute(Guid questionId, byte[] publicKey)
		{
			foreach (var block in blockchain.Trunk)
			{
				if (block?.Votes == null) continue;

				foreach (var vote in block.Votes.Where(i => i.QuestionId == questionId && i.PublicKey.SequenceEqual(publicKey)))
					yield return vote;
			}
		}
	}
}