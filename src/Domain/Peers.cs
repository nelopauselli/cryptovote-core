using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Channels;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;

namespace Domain
{
	public class Peers
	{
		public const int TimeoutMillicesonds = 10000;
		private readonly IList<IPeer> hosts = new List<IPeer>();
		private readonly string nodeName;
		private readonly INodeLogger logger;

		private readonly IList<Task> tasks = new List<Task>();

		public Peers(string nodeName, INodeLogger logger)
		{
			this.nodeName = nodeName;
			this.logger = logger;
		}

		public IEnumerable<IPeer> Hosts => hosts;
		public int Count => hosts.Count;

		public void Add(IPeer peer)
		{
			hosts.Add(peer);
		}

		public void Broadcast(Vote vote)
		{
			Broadcast(new SendVoteMessage(vote));
		}

		public void Broadcast(Question question)
		{
			Broadcast(new SendQuestionMessage(question));
		}

		public void Broadcast(Community community)
		{
			Broadcast(new SendCommunityMessage(community));
		}

		public void Broadcast(Member member)
		{
			Broadcast(new SendMemberMessage(member));
		}
		public void Broadcast(Document document)
		{
			Broadcast(new SendDocumentMessage(document));
		}

		public void Broadcast(Block block)
		{
			Broadcast(new SendBlockMessage(block));
		}

		public void GetBlockByHash(byte[] hash)
		{
			Broadcast(new BlockQueryMessage(hash));
		}

		public void Broadcast(Fiscal fiscal)
		{
			Broadcast(new SendFiscalMessage(fiscal));
		}

		public void Broadcast(Urn urn)
		{
			Broadcast(new SendUrnMessage(urn));
		}

		public void Broadcast(Recount recount)
		{
			Broadcast(new SendRecountMessage(recount));
		}

		public void Broadcast(ProtocolMessage message)
		{
			if (hosts.Count == 0)
			{
				logger.Warning($"El nodo '{nodeName}' no tiene pareas a quienes enviar el comando '{Encoding.UTF8.GetString(message.GetBytes())}'");
				return;
			}

			logger.Information($"Enviando comando '{Encoding.UTF8.GetString(message.GetBytes())}' desde el nodo '{nodeName}' a {hosts.Count} nodos");

			var data = message.GetBytes();

			var peers = hosts.ToArray();
			foreach (var peer in peers)
			{

				Task task = Send(peer, data);
				tasks.Add(task);
			}

			//Task.WaitAll(tasks.ToArray());
		}

		public Task Send(IPeer peer, ProtocolMessage message)
		{
			logger.Debug($"Enviando mensaje al nodo #{peer}: {Encoding.UTF8.GetString(message.GetBytes())}");
			return Send(peer, message.GetBytes());
		}

		private async Task Send(IPeer peer, byte[] data)
		{
			try
			{
				if (peer.IsReady)
				{
					using (var cts = new CancellationTokenSource(TimeoutMillicesonds))
					{
						await peer.SendAsync(data, cts.Token);
						if (!cts.IsCancellationRequested)
							logger.Debug($"Mensaje enviado al nodo #{peer}");
						else
							logger.Warning($"ERROR enviando mensaje al nodo #{peer}");
					}
				}
				else
				{
					logger.Debug($"El socket #{peer} no está abierto");
				}
			}
			catch (ChannelException ex)
			{
				logger.Warning(message: ex.ToString());
			}
		}

		public bool Contains(IPeer peer)
		{
			return hosts.Any(p => p.Id == peer.Id);
		}
	}
}