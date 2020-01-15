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

		public Guid NodeId => Guid.TryParse(configuration["NodeId"], out var value) ? value : Guid.NewGuid();
		public string NodeName => configuration["NodeName"] ?? "nodo sin nombre";
		public string NodePublicUrl => configuration["NodePublicUrl"];

		public string PeerUrl => configuration["PeerUrl"];

		public byte BlockchainDificulty => (byte)(byte.TryParse(configuration["BlockchainDificulty"] ?? "1", out var value) ? value : 1);

		public string MinerAddress => configuration["MinerAddress"];

		public int MinerInterval => int.TryParse(configuration["MinerInterval"] ?? "10000", out var value) ? value : 10000;
	}
}