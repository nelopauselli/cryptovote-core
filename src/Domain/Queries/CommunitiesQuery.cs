using System.Collections.Generic;
using Domain.Elections;

namespace Domain.Queries
{
	public class CommunitiesQuery
	{
		private readonly Blockchain blockchain;

		public CommunitiesQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IEnumerable<Community> Execute()
		{
			foreach (var block in blockchain.Trunk)
			{
				if (block?.Communities == null) continue;

				foreach (var community in block.Communities)
					yield return community;
			}
		}
	}
}