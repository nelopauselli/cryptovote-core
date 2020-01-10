using System;
using System.Linq;
using Domain.Elections;

namespace Domain.Queries
{
	public class MemberQuery
	{
		private readonly Blockchain blockchain;

		public MemberQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public Member Execute(Guid communityId, Guid memberId)
		{
			foreach (var block in blockchain.Trunk)
			{
				if (block?.Members== null) continue;

				foreach (var question in block.Members.Where(i => i.CommunityId == communityId && i.Id == memberId))
					return question;
			}

			return null;
		}
	}
}