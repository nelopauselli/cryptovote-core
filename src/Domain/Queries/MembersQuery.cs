﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Elections;

namespace Domain.Queries
{
	public class MembersQuery
	{
		private readonly Blockchain blockchain;

		public MembersQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IEnumerable<Member> Execute(Guid communityId)
		{
			foreach (var block in blockchain.Trunk)
			{
				foreach (var members in block.Members.Where(i => i.CommunityId == communityId))
					yield return members;
			}
		}
	}
}