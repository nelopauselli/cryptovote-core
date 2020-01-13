namespace Domain.Queries
{
	public class LastBlockQuery
	{
		private readonly Blockchain blockchain;

		public LastBlockQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public Block Execute()
		{
			return blockchain.Last;
		}
	}
}