using System;

namespace Domain
{
	public interface INodeConfiguration
	{
		Guid NodeId { get; }
		string NodeName { get; }
		string NodePublicUrl { get; }
		byte BlockchainDificulty { get; }
		string MinerAddress { get; }
		int MinerInterval { get; }
		string PeerUrl { get; }
	}
}