using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Crypto;
using Newtonsoft.Json;

namespace Client
{
	public class Worker : IDisposable
	{
		private readonly AppConfig config;
		private readonly ILogger logger;
		private readonly WebSocketChannel channel;
		private readonly Screen screen;
		private readonly SignatureVerify signatureVerify;
		private readonly Blockchain blockchain;
		private readonly IStorage pendings;
		private readonly KeysPair minerKeys;

		public Worker(AppConfig config)
		{
			this.config = config ?? throw new ArgumentNullException(nameof(config));

			// TODO: levantar las claves del minero desde un archivo
			minerKeys = Blockchain.CryptoService.GeneratePair();

			logger = new MemoryLogger();
			channel = new WebSocketChannel(config.WebSocketUrl, logger);
			blockchain = new Blockchain(minerKeys.PublicKey, 2);
			
			var counter = new Counter(blockchain);

			pendings = new MemoryStorage();
			screen = new Screen(logger, config, blockchain, pendings, channel, counter);

			signatureVerify = new SignatureVerify(Blockchain.CryptoService);
		}

		public void Work()
		{
			// TODO: conectarse a la red para descargar la blockchain existente
			blockchain.LoadGenesisBlock("genesis.block");

			long nextTimeToMine = DateTimeOffset.Now.AddSeconds(15).ToUnixTimeSeconds();
			screen.Draw(nextTimeToMine);

			try
			{
				var loadingTask = LoadHistory(config.RestUrl);
				loadingTask.Wait(WebSocketChannel.TimeOutMilliseconds);
			}
			catch (Exception ex)
			{
				logger.Error("Error cargando historial", ex);
			}

			channel.IncommingVote += (sender, vote) => Add(vote);
			channel.IncommingTopic += (sender, topic) => Add(topic);
			channel.IncommingOrganization += (sender, organization) => Add(organization);
			
			logger.Debug("Init loop");

			int count = 0;
			Task taskConnect = null;
			while (true)
			{
				if (nextTimeToMine < DateTimeOffset.Now.ToUnixTimeSeconds())
				{
					var transactions = pendings.Transactions.ToArray();
					if (transactions.Length > 0)
					{
						logger.Info($"Mining next block with {transactions.Length} transactions");

						var block = blockchain.MineNextBlock(transactions);
						
						foreach (var item in block.GetTransactions())
							pendings.Transactions.Remove(item);
					}

					nextTimeToMine = DateTimeOffset.Now.AddSeconds(30).ToUnixTimeSeconds();
				}

				if (taskConnect == null || taskConnect.IsCompleted)
					taskConnect = channel.Connect();

				if (count % 10 == 0)
				{
					screen.Draw(nextTimeToMine);
				}

				if (count == int.MaxValue) count = 0;
				else count++;

				Thread.Sleep(100);
			}
		}

		private async Task LoadHistory(Uri restApiUri)
		{
			logger.Debug("Getting history");
			using (var restClient = new HttpClient())
			{
				restClient.DefaultRequestHeaders.Accept.Clear();
				restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				restClient.DefaultRequestHeaders.Add("User-Agent", "Client App");

				try
				{
					var url = new Uri(restApiUri, "api/organization");
					logger.Debug($"Buscando organizaciones en {url}");
					var response = await restClient.GetStringAsync(url);
					var organizations = JsonConvert.DeserializeObject<Organization[]>(response);

					foreach (var organization in organizations)
					{
						Add(organization);
					}
				}
				catch (Exception ex)
				{
					logger.Error(ex.Message, ex);
				}

				try
				{
					var url = new Uri(restApiUri, "api/vote");
					logger.Debug($"Buscando votyos en {url}");
					var response = await restClient.GetStringAsync(url);
					var votes = JsonConvert.DeserializeObject<Vote[]>(response);

					foreach (var vote in votes)
					{
						Add(vote);
					}
				}
				catch (Exception ex)
				{
					logger.Error(ex.Message, ex);
				}

			}
		}

		private void Add(Vote vote)
		{
			pendings.Incomming++;

			if (signatureVerify.Verify(vote))
				pendings.VotesAdd(vote);
		}

		private void Add(Topic topic)
		{
			if (signatureVerify.Verify(topic))
				pendings.TopicAdd(topic);
		}

		private void Add(Organization organization)
		{
			if (signatureVerify.Verify(organization))
				pendings.OrganizationAdd(organization);
		}

		public void Dispose()
		{
			channel?.Dispose();
		}
	}
}