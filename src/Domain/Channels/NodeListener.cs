using Domain.Elections;

namespace Domain.Channels
{
	public class NodeListener : IEventListener
	{
		private readonly INode node;

		public NodeListener(INode node)
		{
			this.node = node;
		}

		public void Incomming(Recount recount)
		{
			node.Add(recount);
		}

		public void Incomming(Fiscal fiscal)
		{
			node.Add(fiscal);
		}

		public void Incomming(Urn urn)
		{
			node.Add(urn);
		}

		public void Incomming(Vote vote)
		{
			node.Add(vote);
		}

		public void Incomming(Issue issue)
		{
			node.Add(issue);
		}

		public void Incomming(Member member)
		{
			node.Add(member);
		}

		public void Incomming(Community community)
		{
			node.Add(community);
		}

		public void Incomming(Document document)
		{
			node.Add(document);
		}

		public void Incomming(Block block)
		{
			node.Add(block);
		}

		public void Incomming(IPeer peer)
		{
			node.Register(peer.Host, peer.Port);
		}

		public void Debug(string message)
		{
			// Nada que hacer
		}

		public void Information(string message)
		{
			// Nada que hacer
		}

		public void Warning(string message)
		{
			// Nada que hacer
		}

		public void Error(string message)
		{
			// Nada que hacer
		}
	}
}