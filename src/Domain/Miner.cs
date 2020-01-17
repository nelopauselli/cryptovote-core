using System;
using System.Security.Cryptography;
using System.Threading;

namespace Domain
{
	public class Miner
	{
		private readonly byte[] publicKey;
		private bool stop;

		public Miner(byte[] publicKey)
		{
			this.publicKey = publicKey;
		}

		public bool Mine(Block block, byte dificulty)
		{
			block.MinerPublicKey = publicKey;
			block.Dificulty = dificulty;

			var data = block.GetData();

			stop = false;
			while (!stop)
			{
				using (var sha256 = new SHA256Managed())
				{
					sha256.Initialize();
					block.Hash = sha256.ComputeHash(data);
				}

				var first = Array.FindIndex(block.Hash, b => b != 0);
				if (first >= dificulty)
				{
					return true;
				}

				block.Nonce++;
				Buffer.BlockCopy(BitConverter.GetBytes(block.Nonce), 0, data, data.Length - 4, 4);

				//Thread.Sleep(1);
			}

			return false;
		}

		public void Stop()
		{
			stop = true;
		}
	}
}