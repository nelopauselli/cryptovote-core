using Domain;

namespace Tests.Mocks
{
	public class NodeConfiguration : INodeConfiguration
	{
		public NodeConfiguration(string name, string minerAddress, byte blockchainDificulty, int minerInterval, int port)
		{
			Name = name;
			BlockchainDificulty = blockchainDificulty;
			MinerAddress = minerAddress;
			MinerInterval = minerInterval;
			MyHost = "127.0.0.1";
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