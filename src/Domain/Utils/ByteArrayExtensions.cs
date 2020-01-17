using System;
using System.Text;

namespace Domain.Utils
{
	public static class ByteArrayExtensions
	{
		public static string ByteArrayToHexString(this byte[] bytes)
		{
			if (bytes == null) return null;

			StringBuilder result = new StringBuilder(bytes.Length * 2);
			const string hexAlphabet = "0123456789abcdef";

			foreach (byte b in bytes)
			{
				result.Append(hexAlphabet[b >> 4]);
				result.Append(hexAlphabet[b & 0xF]);
			}

			return result.ToString();
		}

		public static byte[] HexStringToByteArray(this string raw)
		{
			byte[] Bytes = new byte[raw.Length / 2];
			int[] HexValue = new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05,
				0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

			for (int x = 0, i = 0; i < raw.Length; i += 2, x += 1)
			{
				Bytes[x] = (byte)(HexValue[Char.ToUpper(raw[i + 0]) - '0'] << 4 |
				                  HexValue[Char.ToUpper(raw[i + 1]) - '0']);
			}

			return Bytes;
		}
	}
}