using System;
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

		public Guid NodeId => Guid.TryParse(configuration["Node:Id"], out var value) ? value : Guid.NewGuid();
		public string NodeName => configuration["Node:Name"] ?? "nodo sin nombre";
		public string NodePublicUrl => configuration["Node:PublicUrl"] ?? "localhost:8000";

		public byte BlockchainDificulty => (byte)(byte.TryParse(configuration["Blockchain:Dificulty"] ?? "1", out var value) ? value : 1);

		public string MinerAddress => configuration["Miner:Address"];

		public int MinerInterval => int.TryParse(configuration["Miner:Interval"] ?? "10000", out var value) ? value : 10000;

	}
}