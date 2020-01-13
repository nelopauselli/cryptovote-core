using System;
using Domain.Elections;

namespace Domain.Queries
{
	public class QuestionResultQuery
	{
		private readonly Blockchain blockchain;

		public QuestionResultQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public QuestionResult Execute(Guid communityId, Guid questionId)
		{
			return blockchain.GetResult(questionId);
		}
	}
}