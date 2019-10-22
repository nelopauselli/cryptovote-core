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

		public Question Execute(string content)
		{
			var chunks = content.Split('#');

			if (chunks.Length != 2)
				return null;
			if (!Guid.TryParse(chunks[0], out var communityId))
				return null;
			if (!Guid.TryParse(chunks[1], out var questionId))
				return null;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var question in block.Questions.Where(i => i.CommunityId == communityId && i.Id == questionId))
					return question;
			}

			return null;
		}
	}
}