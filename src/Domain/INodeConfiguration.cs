namespace Domain
{
	public interface INodeConfiguration
	{
		string Name { get; }
		byte BlockchainDificulty { get; }
		string MinerAddress { get; }
		int MinerInterval { get; }
		string PublicUrl { get; }
	}
}