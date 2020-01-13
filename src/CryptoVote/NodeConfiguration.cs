using System;
using Domain;
using Microsoft.Extensions.Configuration;

namespace CryptoVote
{
	class NodeConfiguration : INodeConfiguration
	{
		private readonly IConfiguration configuration;

		public NodeConfiguration(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public string Name => configuration["Name"];

		public byte BlockchainDificulty =>
			byte.TryParse(configuration["Blockchain:Dificulty"], out var dificulty)
				? dificulty
				: (byte) 2;

		public string MinerAddress => configuration["Miner:Address"];

		public int MinerInterval =>
			int.TryParse(configuration["Miner:Interval"], out var interval)
				? interval
				: 60 * 1000;

		public string PublicUrl => configuration["My:PublicUrl"];

		public string MyHost => configuration["My:Host"];

		public string PeerUrl => configuration["Peer:Host"];

		public bool ConsoleColored => configuration["Console:Colored"]?.ToLower() == "true" || configuration["Console:Colored"] == "1";

		public VerbosityEnum Verbosity => Enum.TryParse(configuration["Verbosity"]?.ToUpper(), out VerbosityEnum verbosity) ? verbosity : VerbosityEnum.INFO;
	}
}