﻿using Domain;
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
		public string MinerAddress => configuration["Miner:Address"];
		public string BlockchainDificulty => configuration["Blockchain:Dificulty"];
		public string MinerInterval => configuration["Miner:Interval"];

		public string MyHost => configuration["My:Host"];

		public int MyPort => 
			int.TryParse(configuration["My:Port"], out var port)
				? port
				: 13000;

		public string PeerHost => configuration["Peer:Host"];

		public int PeerPort =>
			int.TryParse(configuration["Peer:Port"], out var port)
				? port
				: 13000;

		public bool ConsoleColored => configuration["Console:Colored"]?.ToLower() == "true" || configuration["Console:Colored"] == "1";
	}
}