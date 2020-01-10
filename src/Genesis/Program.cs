using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Domain;
using Domain.Converters;
using Domain.Crypto;
using Domain.Utils;

namespace Genesis
{
	class Program
	{
		static void Main(string[] args)
		{
			var owner = new KeysPair(
				Base58.Decode("5eGEt2wuDrvEt1d7zUDdqZEF9u2sYiVXg6jJEk5G1ytJ"),
				Base58.Decode("QXuwUXRhANuouMkHTc2tbcZhkcucShZQKEhs7XSfseSd6Rq8q2G3sSZc1Q1z5jdj4Nz8dQuieiiyDVLiKWDmtJVp"));

			Console.WriteLine("Leyendo White Paper");
			var content = File.ReadAllText("white-paper.md");
			var whitePaper = new Document(content);

			Console.WriteLine("Inicializando Firmador");
			var signer = new Signer(CryptoService.Instance);

			Console.WriteLine("Firmando White Paper");
			signer.Sign(whitePaper, owner);

			Console.WriteLine("Construyendo bloque Génesis");
			var block = new Block(Array.Empty<byte>(), 0);
			block.Documents.Add(whitePaper);

			byte dificulty = 3;
			Console.WriteLine($"Minando bloque Génesis con una dificultad de {dificulty}");
			var miner = new Miner(owner.PublicKey);
			miner.Mine(block, dificulty);

			Console.WriteLine("Serializando bloque Génesis");
			var json = JsonSerializer.Serialize(block, JsonDefaultSettings.Options);

			Console.WriteLine("Guardando bloque minado");
			File.WriteAllText("genesis.block", json, Encoding.UTF8);

			Console.WriteLine("Listo ;)");

		}
	}
}
