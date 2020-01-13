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
		public string Name { get; set; } = "localhost";

		public byte BlockchainDificulty { get; set; } = 1;

		public string MinerAddress { get; set; }

		public int MinerInterval { get; set; } = 10000;

		public string PublicUrl { get; set; } = "localhost:8000";
	}
}