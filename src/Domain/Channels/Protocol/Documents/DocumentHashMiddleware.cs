using System;

namespace Domain.Channels.Protocol.Documents
{
	public class DocumentHashMiddleware : IMiddleware
	{
		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.DocumentHash) return false;

			var buffer = new byte[16];
			peer.Read(buffer, 0, buffer.Length);
			peer.History.Add(BitConverter.ToString(buffer));

			return true;
		}
	}
}