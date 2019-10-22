using System;
using System.Linq;

namespace Domain
{
	public class Counter
	{
		private readonly Blockchain blockchain;

		public Counter(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public int TotalFor(Guid choiceId)
		{
			var votes = blockchain.Trunk.SelectMany(c => c.Votes).ToArray();

			var addresses = votes.Select(v => v.PublicKey).ToList();

			int result = 0;
			while(addresses.Count>0)
			{
				var address = addresses[0];

				var vote = votes.LastOrDefault(v => v.PublicKey == address);
				if (vote != null)
				{
					if (vote.ChoiceId == choiceId) result++;
				}

				addresses.RemoveAll(pk => pk.SequenceEqual(address));
			}

			return result;
		}
	}
}