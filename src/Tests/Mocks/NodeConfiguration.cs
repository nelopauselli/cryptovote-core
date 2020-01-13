using System;
using Domain;

namespace Tests.Mocks
{
	public class NodeConfiguration : INodeConfiguration
	{
		public NodeConfiguration(string name, string minerAddress, byte blockchainDificulty, int minerInterval, string publicUrl)
		{
			NodeId = Guid.NewGuid();
			NodeName = name;
			NodePublicUrl = publicUrl;
			BlockchainDificulty = blockchainDificulty;
			MinerAddress = minerAddress;
			MinerInterval = minerInterval;
		}

		public Guid NodeId { get; }
		public string NodeName { get; }
		public string NodePublicUrl { get; }
		public byte BlockchainDificulty { get; }
		public string MinerAddress { get; }
		public int MinerInterval { get; }
	}
}