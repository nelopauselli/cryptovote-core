using System;
using System.Text;

namespace Domain
{
	public class Document : BlockItem
	{
		public string Text { get; }
		public DateTimeOffset Time { get; }

		public Document(string text)
		{
			Text = text;
			Time = DateTimeOffset.UtcNow;
		}
		
		public override byte[] GetData()
		{
			var data = new byte[20 + 8];

			Buffer.BlockCopy(Encoding.UTF8.GetBytes(Text.PadRight(20)), 0, data, 0, 20);

			var time = BitConverter.GetBytes(Time.ToUnixTimeMilliseconds());
			Array.Reverse(time);
			Buffer.BlockCopy(time, 0, data, 20, 8);

			return data;
		}

		public override string GetKey()
		{
			return Text;
		}
	}
}