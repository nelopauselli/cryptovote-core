using System;

namespace Domain.Elections
{
	public class BlockBuilder : IBlockBuilder
	{
		public Block BuildNextBlock(BlockItem[] transactions, Block previous, byte dificulty)
		{
			var previousHash = previous?.Hash ?? Array.Empty<byte>();
			var number = (previous?.BlockNumber + 1) ?? 0;

			var block = new Block(previousHash, number);

			foreach (var transaction in transactions)
			{
				if (transaction.GetType() == typeof(Document))
					block.Documents.Add(transaction as Document);
				else if (transaction.GetType() == typeof(Community))
					block.Communities.Add(transaction as Community);
				else if (transaction.GetType() == typeof(Member))
					block.Members.Add(transaction as Member);
				else if (transaction.GetType() == typeof(Issue))
					block.Issues.Add(transaction as Issue);
				else if (transaction.GetType() == typeof(Urn))
					block.Urns.Add(transaction as Urn);
				else if (transaction.GetType() == typeof(Fiscal))
					block.Fiscals.Add(transaction as Fiscal);
				else if (transaction.GetType() == typeof(Vote))
					block.Votes.Add(transaction as Vote);
				else if (transaction.GetType() == typeof(Recount))
					block.Recounts.Add(transaction as Recount);
				else if (transaction.GetType() == typeof(Recognition))
					block.Recognitions.Add(transaction as Recognition);
				else
					throw new ApplicationException($"The type '{transaction.GetType().Name}' is unknown");
			}

			return block;
		}
	}
}