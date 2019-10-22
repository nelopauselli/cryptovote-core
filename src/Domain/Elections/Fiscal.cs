using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Utils;

namespace Domain.Elections
{
	public class Fiscal : BlockItem
	{
		public Guid Id { get; set; }
		public Guid IssueId { get; set; }
		public Guid ChoiceId { get; set; }
		public byte[] Address { get; set; } = Array.Empty<byte>();

		public override string GetKey()
		{
			return BuildKey(IssueId, ChoiceId, Id);
		}

		public static string BuildKey(Guid issueId, Guid choiceId, Guid fiscalId)
		{
			return $"{issueId:n}:{choiceId:n}:{fiscalId:n}";
		}

		public override byte[] GetData()
		{
			var data = new byte[16 + 16 + 16];

			Buffer.BlockCopy(IssueId.ToOrderByteArray(), 0, data, 0, 16);
			Buffer.BlockCopy(ChoiceId.ToOrderByteArray(), 0, data, 16, 16);
			Buffer.BlockCopy(Id.ToOrderByteArray(), 0, data, 32, 16);

			return data;
		}

		public override bool IsValid(IList<Block> chain)
		{
			if (!base.IsValid(chain)) return false;
			var registered = chain.Any(b => b.Issues.Any(i => i.Id == IssueId));
			return registered;
		}
	}
}