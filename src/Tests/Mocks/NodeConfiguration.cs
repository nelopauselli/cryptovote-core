using Domain;

namespace Tests.Mocks
{
	public class NodeConfiguration : INodeConfiguration
	{
		public NodeConfiguration(string name, string minerAddress, byte blockchainDificulty, int minerInterval, int port, string host= "127.0.0.1")
		{
			Name = name;
			BlockchainDificulty = blockchainDificulty;
			MinerAddress = minerAddress;
			MinerInterval = minerInterval;
			MyHost = host;
			MyPort = port;
		}

		public string Name { get; }
		public byte BlockchainDificulty { get; }
		public string MinerAddress { get; }
		public int MinerInterval { get; }
		public string MyHost { get; }
		public int MyPort { get; }
	}
}