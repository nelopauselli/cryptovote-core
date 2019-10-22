using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Utils;

namespace Domain.Scrutiny
{
	public class Member : BlockItem
	{
		public Guid Id { get; set; }
		public Guid CommunityId { get; set; }
		public string Name { get; set; }
		public byte[] Address { get; set; } = Array.Empty<byte>();

		public override byte[] GetData()
		{
			var nameAsBytes = Encoding.UTF8.GetBytes(Name);

			var data = new byte[16 + 16 + Address.Length + nameAsBytes.Length];

			Buffer.BlockCopy(Id.ToOrderByteArray(), 0, data, 0, 16);
			Buffer.BlockCopy(CommunityId.ToOrderByteArray(), 0, data, 16, 16);
			Buffer.BlockCopy(Address, 0, data, 32, Address.Length);
			Buffer.BlockCopy(nameAsBytes, 0, data, 32+ Address.Length, nameAsBytes.Length);

			return data;
		}

		public override string GetKey()
		{
			return BuildKey(CommunityId, Id);
		}

		public static string BuildKey(Guid communityId, Guid memberId)
		{
			return $"{communityId:n}:{memberId}";
		}

		public override bool IsValid(IList<Block> chain)
		{
			if (!base.IsValid(chain)) return false;
			
			var registered = chain.Any(b =>b.Communities.Any(t =>t.Address.SequenceEqual(PublicKey)));

			return registered;
		}
	}
}