using System;

namespace Domain
{
	public class NodeOptions
	{
		public NodeOptions()
		{
			NodeId = Guid.NewGuid();
		}

		public NodeOptions(string name, string minerAddress, byte blockchainDificulty, int minerInterval, string publicUrl)
		{
			NodeName = name;
			NodePublicUrl = publicUrl;
			BlockchainDificulty = blockchainDificulty;
			MinerAddress = minerAddress;
			MinerInterval = minerInterval;
		}

		public Guid NodeId { get; set; }
		public string NodeName { get; set; } = "nodo sin nombre";
		public string NodePublicUrl { get; set; } 
		public byte BlockchainDificulty { get; set; } = 1;
		public string MinerAddress { get; set; }
		public int MinerInterval { get; set; } = 10000;
		public string PeerUrl { get; set; }
		public int PeersCheckInterval { get; set; } = 60000;
		public int SyncronizeInterval { get; set; } = 30000;
	}
}