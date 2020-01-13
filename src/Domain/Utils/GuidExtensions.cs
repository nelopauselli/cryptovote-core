using System;

namespace Domain.Utils
{
	// Implements https://en.bitcoin.it/wiki/Base58Check_encoding
	public static class GuidExtensions
	{
		public static byte[] ToOrderByteArray(this Guid guid)
		{
			var buf = guid.ToByteArray();
			
			var a = new byte[4];
			Array.Copy(buf, 0, a, 0, 4);
			Array.Reverse(a);

			var b = new byte[2];
			Array.Copy(buf, 4, b, 0, 2);
			Array.Reverse(b);

			var c = new byte[2];
			Array.Copy(buf, 6, c, 0, 2);
			Array.Reverse(c);

			var d = new byte[8];
			Array.Copy(buf, 8, d, 0, 8);

			var ret = new byte[16];
			Buffer.BlockCopy(a, 0, ret, 0, 4);
			Buffer.BlockCopy(b, 0, ret, 4, 2);
			Buffer.BlockCopy(c, 0, ret, 6, 2);
			Buffer.BlockCopy(d, 0, ret, 8, 8);

			return ret;
		}

		public static Guid ToGuid(this byte[] buf)
		{
			Array.Reverse(buf, 0, 4);
			Array.Reverse(buf, 4, 2);
			Array.Reverse(buf, 6, 2);

			return new Guid(buf);
		}
	}
}