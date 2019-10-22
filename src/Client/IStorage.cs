using System.Collections.Generic;
using Domain;

namespace Client
{
	public interface IStorage
	{
		IList<BlockItem> Transactions { get; }
		int Incomming { get; set; }
		Vote LastVote { get;  }
		void VotesAdd(Vote vote);
		void TopicAdd(Topic topic);
		void OrganizationAdd(Organization organization);
	}
}