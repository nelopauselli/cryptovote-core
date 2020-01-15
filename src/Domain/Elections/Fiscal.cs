using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Utils;

namespace Domain.Elections
{
	public class Fiscal : BlockItem
	{
		public Guid Id { get; set; }
		public Guid QuestionId { get; set; }
		public Guid ChoiceId { get; set; }
		public byte[] Address { get; set; } = Array.Empty<byte>();

		public override string GetKey()
		{
			return BuildKey(QuestionId, ChoiceId, Id);
		}

		public static string BuildKey(Guid questionId, Guid choiceId, Guid fiscalId)
		{
			return $"{questionId:n}:{choiceId:n}:{fiscalId:n}";
		}

		public override byte[] GetData()
		{
			var data = new byte[16 + 16 + 16];

			Buffer.BlockCopy(QuestionId.ToOrderByteArray(), 0, data, 0, 16);
			Buffer.BlockCopy(ChoiceId.ToOrderByteArray(), 0, data, 16, 16);
			Buffer.BlockCopy(Id.ToOrderByteArray(), 0, data, 32, 16);

			return data;
		}

		public override bool IsValid(IList<Block> chain)
		{
			if (!base.IsValid(chain)) return false;
			var registered = chain.Any(b => b.Questions != null && b.Questions.Any(i => i.Id == QuestionId));
			if (!registered)
				Messages.Add($"No existe la question {QuestionId}");

			return registered;
		}
	}
}