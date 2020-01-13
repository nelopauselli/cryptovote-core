using System;

namespace Domain.Queries
{
	public class DocumentQuery
	{
		private readonly Blockchain blockchain;

		public DocumentQuery(Blockchain blockchain)
		{
			this.blockchain = blockchain;
		}

		public Document Execute(Guid id)
		{
			throw new NotImplementedException();
			//foreach (var block in blockchain.Trunk)
			//{
			//	if (block?.Documents == null) continue;

			//	foreach (var document in block.Documents)
			//		if (document.Id == id)
			//			return document;
			//}

			//return null;
		}
	}
}