using System.Security.Cryptography;

namespace Domain.Channels.Protocol.Documents
{
	public class DocumentMiddleware : IMiddleware
	{
		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.Document) return false;

			// TODO: hacer algo con el documento
			var buffer = new byte[header.Length];

			var count = 0;
			while (count < header.Length)
			{
				count += peer.Read(buffer, count, buffer.Length);
			}

			byte[] hash;
			using (var md5 = MD5.Create())
			{
				hash = md5.ComputeHash(buffer);
			}

			peer.Send(new DocumentHashCommand(hash));

			return true;
		}
	}
}