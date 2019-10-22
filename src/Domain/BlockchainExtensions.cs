using System.IO;
using Newtonsoft.Json;

namespace Domain
{
	public static class BlockchainExtensions
	{
		public static void LoadGenesisBlock(this Blockchain blockchain, string path)
		{
			var json = File.ReadAllText(path);
			if (json != null)
			{
				var genesis = JsonConvert.DeserializeObject<Block>(json);
				blockchain.AddBlock(genesis);
			}
		}
	}
}