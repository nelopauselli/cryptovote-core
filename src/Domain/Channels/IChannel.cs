using System.Collections.Generic;

namespace Domain.Channels
{
	public interface IChannel
	{
		string ID { get; }
		string ListenHost { get; }
		int ListenPort { get; }
		IEnumerable<TcpPeer> Peers { get; }
		INode Node { get; }
		
		void Connect(PeerInfo peerInfo);
	}
}