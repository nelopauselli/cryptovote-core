using System;
using Domain;
using Domain.Utils;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class Miner_tests
	{
		[Test]
		public void Mine()
		{
			var miner = new Miner(new byte[] {1, 2, 3, 4});

			var block = new Block(Array.Empty<byte>(), 1);
			miner.Mine(block, 2);

			Assert.AreEqual(0, block.Hash[0]);
			Assert.AreEqual(0, block.Hash[1]);

			Console.WriteLine(block.Hash.ByteArrayToHexString());
			Console.WriteLine(Base58.Encode(block.Hash));
			Assert.IsTrue(block.Hash.ByteArrayToHexString().StartsWith("0000"));
		}
	}
}