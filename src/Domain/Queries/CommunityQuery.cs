using System;
using Domain.Elections;

namespace Domain.Queries
{
	public class CommunityQuery
	{
		private readonly Blockchain blockchain;

		public CommunityQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public Community Execute(string content)
		{
			if (!Guid.TryParse(content, out var communityId))
				return null;

			foreach (var block in blockchain.Trunk)
			{
				foreach (var community in block.Communities)
					if (community.Id == communityId)
						return community;
			}

			return null;
		}
	}
}