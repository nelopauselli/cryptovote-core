using System;
using System.Linq;
using Domain.Elections;

namespace Domain.Queries
{
	public class RecountQuery
	{
		private readonly Blockchain blockchain;

		public RecountQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public Recount Execute(Guid urnId)
		{
			foreach (var block in blockchain.Trunk)
			{
				var recount = block?.Recounts?.FirstOrDefault(i => i.UrnId == urnId);
				if (recount != null)
					return recount;
			}

			return null;
		}

	}
}