using System;
using System.Linq;
using Domain.Elections;

namespace Domain.Queries
{
	public class QuestionQuery
	{
		private readonly Blockchain blockchain;

		public QuestionQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public Question Execute(Guid communityId, Guid questionId)
		{
			foreach (var block in blockchain.Trunk)
			{
				if (block?.Questions == null) continue;

				foreach (var question in block.Questions.Where(i => i.CommunityId == communityId && i.Id == questionId))
					return question;
			}

			return null;
		}
	}
}