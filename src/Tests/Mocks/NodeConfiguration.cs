using Domain;

namespace Tests.Mocks
{
	public class NodeConfiguration : INodeConfiguration
	{
		public NodeConfiguration(string name, string minerAddress, string blockchainDificulty, string minerInterval, int port)
		{
			Name = name;
			MinerAddress = minerAddress;
			BlockchainDificulty = blockchainDificulty;
			MinerInterval = minerInterval;
			MyHost = "127.0.0.1";
			MyPort = port;
		}

		public string Name { get; }
		public string MinerAddress { get; }
		public string BlockchainDificulty { get; }
		public string MinerInterval { get; }
		public string MyHost { get; }
		public int MyPort { get; }
	}
}