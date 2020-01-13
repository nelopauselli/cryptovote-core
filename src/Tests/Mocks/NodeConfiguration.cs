using Domain;

namespace Tests.Mocks
{
	public class NodeConfiguration : INodeConfiguration
	{
		public NodeConfiguration(string name, string minerAddress, byte blockchainDificulty, int minerInterval, string publicUrl)
		{
			Name = name;
			BlockchainDificulty = blockchainDificulty;
			MinerAddress = minerAddress;
			MinerInterval = minerInterval;
			PublicUrl = publicUrl;
		}

		public string Name { get; }
		public byte BlockchainDificulty { get; }
		public string MinerAddress { get; }
		public int MinerInterval { get; }
		public string PublicUrl { get; }
	}
}