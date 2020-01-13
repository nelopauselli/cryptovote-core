using System.Collections.Generic;
using Domain.Elections;

namespace Domain
{
	public interface IPeerChannel
	{
		void Connect(string myPublicUrl, string targetPublicUrl);
		IList<PeerInfo> ListPeers(string publicUrl);
		Block GetLastBlock(string publicUrl);
		Block GetBlock(string publicUrl, byte[] hash);

		void Send(string publicUrl, Block block);
		void Send(string publicUrl, Community community);
		void Send(string publicUrl, Question question);
		void Send(string publicUrl, Member member);
		void Send(string publicUrl, Document document);
		void Send(string publicUrl, Vote vote);
		void Send(string publicUrl, Fiscal fiscal);
		void Send(string publicUrl, Urn urn);
		void Send(string publicUrl, Recount recount);
	}
}