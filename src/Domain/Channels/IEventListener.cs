using Domain.Elections;

namespace Domain.Channels
{
	public interface IEventListener : INodeLogger
	{
		void Incomming(Recount recount);
		void Incomming(Fiscal fiscal);
		void Incomming(Urn urn);
		void Incomming(Vote vote);
		void Incomming(Issue issue);
		void Incomming(Member member);
		void Incomming(Community community);
		void Incomming(Document document);
		void Incomming(Block block);
		void Incomming(IPeer peer);
	}
}