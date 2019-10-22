using System;
using System.Linq;
using Domain.Utils;
using NUnit.Framework;

namespace Tests.Serealization
{
	public class Guid_bytes_serialization_tests
	{
		[Test]
		public void Serialize()
		{
			var guid = Guid.Parse("51e36016-5cf6-417c-9f42-409d54ceef49");
			var buf = guid.ToOrderByteArray();

			Console.WriteLine(BitConverter.ToString(buf));

			var expected = new byte[] {0x51, 0xe3, 0x60, 0x16, 0x5C, 0xF6, 0x41, 0x7C, 0x9F, 0x42, 0x40, 0x9D, 0x54, 0xCE, 0xEF, 0x49};
			Console.WriteLine(BitConverter.ToString(expected));

			Assert.IsTrue(expected.SequenceEqual(buf));
		}

		[Test]
		public void Parse()
		{
			var guid = new byte[] { 0x51, 0xe3, 0x60, 0x16, 0x5C, 0xF6, 0x41, 0x7C, 0x9F, 0x42, 0x40, 0x9D, 0x54, 0xCE, 0xEF, 0x49 };
			var buf = guid.ToGuid();

			Console.WriteLine(buf.ToString());

			var expected = Guid.Parse("51e36016-5cf6-417c-9f42-409d54ceef49");
			Console.WriteLine(expected.ToString());

			Assert.AreEqual(expected, buf);
		}

		[Test]
		public void Serialize_and_parse()
		{
			var guid1 = Guid.NewGuid();
			var buffer = guid1.ToOrderByteArray();
			var guid2 = buffer.ToGuid();

			Assert.AreEqual(guid1, guid2);
		}
	}
}