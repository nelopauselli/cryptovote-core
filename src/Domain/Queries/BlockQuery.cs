namespace Domain.Queries
{
	public class BlockQuery
	{
		private readonly Blockchain blockchain;

		public BlockQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public Block Execute(byte[] hash)
		{
			return blockchain.GetBlock(hash);
		}
	}
}