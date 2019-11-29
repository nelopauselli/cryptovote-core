using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Channels;

namespace Tests
{
	public class FakeChannel : IChannel
	{
		public FakeChannel(string id, string host, int port)
		{
			ID = id;
			ListenPort = port;
			ListenHost = host;
			Peers = Enumerable.Empty<TcpPeer>();
		}

		public string ID { get; }
		public string ListenHost { get; }
		public int ListenPort { get; }
		public IEnumerable<TcpPeer> Peers { get; }
		public INode Node { get; set; }

		public void Connect(PeerInfo peerInfo)
		{
		}

		public void Discovery()
		{
			
		}
	}
}