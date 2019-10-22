using System;
using System.Collections.Generic;
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

		public Recount Execute(string content)
		{
			if (!Guid.TryParse(content, out var urnId))
				return null;

			foreach (var block in blockchain.Trunk)
			{
				var recount = block.Recounts.FirstOrDefault(i => i.UrnId == urnId);
				if(recount!=null)
					return recount;
			}

			return null;
		}
	}
}