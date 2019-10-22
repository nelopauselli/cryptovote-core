using System;
using System.Text;
using Domain.Utils;

namespace Domain.Scrutiny
{
	public class Choice
	{
		public Guid Id { get; set; }
		public uint Color { get; set; }
		public string Text { get; set; }
		public byte[] GuardianAddress { get; set; } = {0};

		public byte[] GetData()
		{
			var guardianAddress = GuardianAddress ?? new byte[] {0};
			var textEncoded = Encoding.UTF8.GetBytes(Text);

			var data = new byte[16 + 4 + textEncoded.Length + guardianAddress.Length];
			
			Buffer.BlockCopy(Id.ToOrderByteArray(), 0, data, 0, 16);

			var color = BitConverter.GetBytes(Color);
			Array.Reverse(color);
			Buffer.BlockCopy(color, 0, data, 16, 4);

			Buffer.BlockCopy(textEncoded, 0, data, 20, textEncoded.Length);
			Buffer.BlockCopy(guardianAddress, 0, data, 20 + textEncoded.Length, guardianAddress.Length);

			return data;
		}
	}
}