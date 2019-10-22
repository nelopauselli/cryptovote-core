using System;
using System.Text;
using Domain.Utils;

namespace Domain.Scrutiny
{
	public class Community : BlockItem
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public DateTimeOffset CreateAt { get; set; }
		public byte[] Address { get; set; } = Array.Empty<byte>();

		public override byte[] GetData()
		{
			var data = new byte[16 + 20 + 8];
			Buffer.BlockCopy(Id.ToOrderByteArray(), 0, data, 0, 16);
			Buffer.BlockCopy(Encoding.UTF8.GetBytes(Name.PadRight(20)), 0, data, 16, 20);

			var time = BitConverter.GetBytes(CreateAt.ToUnixTimeMilliseconds());
			Array.Reverse(time);
			Buffer.BlockCopy(time, 0, data, 36, 8);

			return data;
		}

		public override string GetKey()
		{
			return BuildKey(Id);
		}

		public static string BuildKey(Guid communityId)
		{
			return $"{communityId:n}";
		}

		public void SetAddress(byte[] address)
		{
			Address = new byte[address.Length];
			Buffer.BlockCopy(address, 0, Address, 0, address.Length);
		}
	}
}