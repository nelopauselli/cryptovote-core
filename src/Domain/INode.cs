using Domain.Elections;

namespace Domain
{
	public interface INode
	{
		Peers Peers { get; }
		Blockchain Blockchain { get; }
		Peer Host { get; }

		void Start();

		void Register(Peer peer);

		void Add(Document document);
		void Add(Community community);
		void Add(Member member);
		void Add(Question question);
		void Add(Vote vote);
		void Add(Fiscal fiscal);
		void Add(Urn urn);
		void Add(Recount recount);
		void Add(Block block);
	}
}