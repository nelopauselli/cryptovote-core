using System;
using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Protocol
{
	public class GetBlockMiddleware : IMiddleware
	{
		private readonly INode node;
		private readonly INodeLogger logger;

		public GetBlockMiddleware(INode node, INodeLogger logger)
		{
			this.node = node;
			this.logger = logger;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.GetBlock) return false;

			var hash = new byte[header.Length];
			peer.Read(hash, 0, header.Length);

			var block = node.Blockchain.GetBlock(hash);
			if (block != null)
			{
				logger.Debug($"Enviando bloque {block.BlockNumber}");
				peer.Send(new SendBlockCommand(block));
			}
			else
			{
				logger.Debug($"No se encontró el bloque {BitConverter.ToString(hash)}");
			}

			return true;
		}
	}
}