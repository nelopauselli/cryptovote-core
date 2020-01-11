using System;
using Domain;

namespace crypto_vote
{
	class NodeConfiguration : INodeConfiguration
	{
		public string Name { get; set; } = "localhost";

		public byte BlockchainDificulty { get; set; } = 1;

		public string MinerAddress { get; set; }

		public int MinerInterval { get; set; } = 10000;

		public string MyName { get; set; } = Guid.NewGuid().ToString("n");

		public string MyHost { get; set; } = "127.0.0.1";

		public int MyPort { get; set; } = 13000;

		public string PeerHost { get; set; } = "127.0.0.1";

		public int PeerPort { get; set; } = 13000;

		public bool ConsoleColored { get; set; } = true;
	}
}