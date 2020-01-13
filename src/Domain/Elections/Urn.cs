using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Utils;

namespace Domain.Elections
{
	public class Urn : BlockItem
	{
		public Guid Id { get; set; }
		public Guid QuestionId { get; set; }
		public string Name { get; set; }
		public byte[][] Authorities { get; set; } = Array.Empty<byte[]>();

		public override string GetKey()
		{
			return BuildKey(QuestionId, Id);
		}

		public static string BuildKey(Guid questionId, Guid urnId)
		{
			return $"{questionId:n}:{urnId:n}";
		}

		public override byte[] GetData()
		{
			var data = new byte[16 + 16 + Name.Length + Authorities.Sum(a => a.Length)];

			Buffer.BlockCopy(QuestionId.ToOrderByteArray(), 0, data, 0, 16);
			Buffer.BlockCopy(Id.ToOrderByteArray(), 0, data, 16, 16);
			Buffer.BlockCopy(Encoding.UTF8.GetBytes(Name), 0, data, 32, Name.Length);

			var offset = 32 + Name.Length;
			foreach (var autoridad in Authorities)
			{
				Buffer.BlockCopy(autoridad, 0, data, offset, autoridad.Length);
				offset += autoridad.Length;
			}

			return data;
		}

		public override bool IsValid(IList<Block> chain)
		{
			if (!base.IsValid(chain)) return false;
			var registered = chain.Any(b => b.Questions != null && b.Questions.Any(i => i.Id == QuestionId));
			return registered;
		}
	}
}