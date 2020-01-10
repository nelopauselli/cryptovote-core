using System.IO;
using System.Text.Json;
using Domain.Converters;

namespace Domain
{
	public static class BlockchainExtensions
	{
		public static void LoadGenesisBlock(this Blockchain blockchain, string path)
		{
			var json = File.ReadAllText(path);
			if (json != null)
			{
				var genesis = JsonSerializer.Deserialize<Block>(json, JsonDefaultSettings.Options);
				blockchain.AddBlock(genesis);
			}
		}
	}
}