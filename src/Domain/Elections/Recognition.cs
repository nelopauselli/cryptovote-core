using System;
using Domain.Utils;

namespace Domain.Elections
{
	public class Recognition : BlockItem
	{
		public Guid Id { get; set; }
		public Guid UrnId { get; set; }
		public byte[] Content { get; set; }
		public override string GetKey()
		{
			return BuildKey(UrnId, Id);
		}

		public static string BuildKey(Guid urnId, Guid recognitionId)
		{
			return $"{urnId:n}:{recognitionId:n}";
		}

		public override byte[] GetData()
		{
			var data = new byte[16 + 16 + Content.Length];

			Buffer.BlockCopy(Id.ToOrderByteArray(), 0, data, 0, 16);
			Buffer.BlockCopy(UrnId.ToOrderByteArray(), 0, data, 16, 16);
			Buffer.BlockCopy(Content, 0, data, 32, Content.Length);
			

			return data;
		}
	}
}