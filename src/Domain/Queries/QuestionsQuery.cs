using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Elections;

namespace Domain.Queries
{
	public class QuestionsQuery
	{
		private readonly Blockchain blockchain;

		public QuestionsQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IEnumerable<Question> Execute(Guid communityId)
		{
			foreach (var block in blockchain.Trunk)
			{
				foreach (var question in block.Questions.Where(i => i.CommunityId == communityId))
					yield return question;
			}
		}
	}
}