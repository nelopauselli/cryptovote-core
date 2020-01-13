using Domain;
using Microsoft.Extensions.Configuration;

namespace crypto_vote
{
	class NodeConfiguration : INodeConfiguration
	{
		private readonly IConfiguration configuration;

		public NodeConfiguration(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		public string Name => configuration["Node:Name"];
		public string PublicUrl => configuration["Node:PublicUrl"] ?? "localhost:8000";

		public byte BlockchainDificulty => (byte)(byte.TryParse(configuration["Blockchain:Dificulty"] ?? "1", out var value) ? value : 1);

		public string MinerAddress => configuration["Miner:Address"];

		public int MinerInterval => int.TryParse(configuration["Miner:Interval"] ?? "10000", out var value) ? value : 10000;

	}
}