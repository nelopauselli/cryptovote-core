using System.Collections.Generic;

namespace Domain.Queries
{
	public class DocumentsQuery
	{
		private readonly Blockchain blockchain;

		public DocumentsQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public IEnumerable<Document> Execute()
		{
			foreach (var block in blockchain.Trunk)
			{
				if (block?.Documents == null) continue;

				foreach (var document in block.Documents)
					yield return document;
			}
		}
	}
}