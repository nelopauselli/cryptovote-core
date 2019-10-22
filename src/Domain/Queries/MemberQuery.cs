using System;
using System.Linq;
using Domain.Scrutiny;

namespace Domain.Queries
{
	public class MemberQuery
	{
		private readonly Blockchain blockchain;

		public MemberQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public Member Execute(string content)
		{
			var chunks = content.Split('#');

			if (chunks.Length != 2)
				return null;
			if (!Guid.TryParse(chunks[0], out var communityId))
				return null;
			if (!Guid.TryParse(chunks[1], out var memberId))
				return null;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var issue in block.Members.Where(i => i.CommunityId == communityId && i.Id == memberId))
					return issue;
			}

			return null;
		}
	}
}