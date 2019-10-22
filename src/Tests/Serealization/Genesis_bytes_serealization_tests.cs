using System;
using System.Text;
using Domain;
using NUnit.Framework;

namespace Tests.Serealization
{
	public class Genesis_bytes_serealization_tests
	{
		private readonly Document genesis = new Document("Genesis Block");

		[Test]
		public void Text_in_data()
		{
			var data = genesis.GetData();

			var name = Encoding.UTF8.GetString(data, 0, 20).Trim();
			Assert.AreEqual(genesis.Text, name);
		}

		[Test]
		public void Time_in_data()
		{
			var data = genesis.GetData();

			var buffer = new byte[8];
			Array.Copy(data, 20, buffer, 0, 8);

			Array.Reverse(buffer);
			var time = DateTimeOffset.FromUnixTimeMilliseconds(BitConverter.ToInt64(buffer));
			Assert.AreEqual(genesis.Time.ToUnixTimeMilliseconds(), time.ToUnixTimeMilliseconds());
		}
	}
}