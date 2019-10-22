using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Utils;

namespace Domain.Scrutiny
{
	public class Recount : BlockItem
	{
		public Guid Id { get; set; }
		public Guid UrnId { get; set; }
		public ChoiceRecount[] Results { get; set; }
		public override string GetKey()
		{
			return BuildKey(UrnId, Id);
		}

		public static string BuildKey(Guid urnId, Guid fiscalId)
		{
			return $"{urnId:n}:{fiscalId:n}";
		}

		public override byte[] GetData()
		{
			var data = new byte[16 + 16 + Results.Length * (16+4)];

			Buffer.BlockCopy(UrnId.ToOrderByteArray(), 0, data, 0, 16);
			Buffer.BlockCopy(Id.ToOrderByteArray(), 0, data, 16, 16);
			for (var index = 0; index < Results.Length; index++)
			{
				var result = Results[index];

				var offset = 32 + index * (16 + 4);
				Buffer.BlockCopy(result.ChoiceId.ToOrderByteArray(), 0, data, offset, 16);

				var votes = BitConverter.GetBytes(result.Votes);
				Array.Reverse(votes);
				Buffer.BlockCopy(votes, 0, data, offset + 16, 4);
			}

			return data;
		}

		public override bool IsValid(IList<Block> chain)
		{
			if (!base.IsValid(chain)) return false;

			var valid = false;
			foreach (var block in chain)
			{
				var urn = block.Urns.SingleOrDefault(u => u.Id == UrnId);
				if (urn != null)
				{
					if (!urn.Authorities.Any(a => a.SequenceEqual(PublicKey)))
						Messages.Add("El firmante no es autoridad en esta urna");
					else
						valid = true;
					break;
				}
			}
			return valid;
		}
	}

	public class ChoiceRecount
	{
		public Guid ChoiceId { get; set; }
		public int Votes { get; set; }
	}
}