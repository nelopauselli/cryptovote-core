using System.Collections.Generic;
using Domain.Elections;

namespace Domain
{
	public class HttpPeerChannel : IPeerChannel
	{
		public void Connect(string myPublicUrl, string targetPublicUrl)
		{
			// TODO: POST a $"{publicUrl}/api/peers/" => {myPublicUrl, targetPublicUrl}
			throw new System.NotImplementedException();
		}

		public IList<PeerInfo> ListPeers(string publicUrl)
		{
			// TODO: GET a $"{publicUrl}/api/peers/"
			throw new System.NotImplementedException();
		}

		public Block GetBlock(string publicUrl, byte[] hash)
		{
			// TODO: GET a $"{publicUrl}/api/chain/{Base58.Encode(hash)}"
			throw new System.NotImplementedException();
		}

		public void Send(string publicUrl, Block block)
		{
			// TODO: POST a $"{publicUrl}/api/chain/" => Json(block)
			throw new System.NotImplementedException();
		}

		public void Send(string publicUrl, Community community)
		{
			// TODO: POST a $"{publicUrl}/api/community/" => Json(community)
			throw new System.NotImplementedException();
		}

		public void Send(string publicUrl, Question question)
		{
			// TODO: POST a $"{publicUrl}/api/question/" => Json(question)
			throw new System.NotImplementedException();
		}

		public Block GetLastBlock(string publicUrl)
		{
			// TODO: GET a $"{publicUrl}/api/chain/"
			throw new System.NotImplementedException();
		}
	}
}