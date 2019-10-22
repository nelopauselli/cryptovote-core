using System.Collections.Generic;
using Domain;

namespace Client
{
	public class MemoryStorage: IStorage
	{
		private readonly List<BlockItem> transactions;

		public MemoryStorage()
		{
			transactions = new List<BlockItem>();
			Incomming = 0;
		}

		public IList<BlockItem> Transactions => transactions;
		public int Incomming { get; set; }

		public Vote LastVote { get; private set; }

		public void VotesAdd(Vote vote)
		{
			transactions.Add(vote);
			LastVote = vote;
		}

		public void TopicAdd(Topic topic)
		{
			transactions.Add(topic);
		}

		public void OrganizationAdd(Organization organization)
		{
			transactions.Add(organization);
		}
	}
}