using System.Net;

namespace Domain
{
	public interface INodeConfiguration
	{
		string Name { get; }
		string MinerAddress { get; }
		string BlockchainDificulty { get; }
		string MinerInterval { get; }
		string MyHost { get; }
		int MyPort { get; }
	}
}