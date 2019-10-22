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

		public QuestionResult Execute(string content)
		{
			var chunks = content.Split('#');

			if (chunks.Length != 2)
				return null;
			if (!Guid.TryParse(chunks[0], out var communityId))
				return null;
			if (!Guid.TryParse(chunks[1], out var questionId))
				return null;

			return blockchain.GetResult(questionId);
		}
	}
}