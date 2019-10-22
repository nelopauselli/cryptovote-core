using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Crypto;
using Domain.Elections;
using Domain.Utils;

namespace DataSample
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Creando datos de ejemplo");

			var crypto = new CryptoSecp256k1();
			var nelo = crypto.GeneratePair(Base58.Decode("2FbceVNGnzuqCFy1M8RLga8R64kAi8Eq6LAhWq52Wdnf")); // Tablet en emulador: me
			var martin = crypto.GeneratePair(Base58.Decode("4x6pChtv9EUNQ27otESf9GyL7PWerKF7z475ESGusnK4")); // Tablet en emulador: martin
			var silvio = crypto.GeneratePair(Base58.Decode("147iFS4LCLJqGqe9W59QSREWvtGgwExKj8vZomZKYojN")); // Tablet en emulador: silvio
			var romina = crypto.GeneratePair(Base58.Decode("13SF2nG7CYsGqY2y1xKP4X318vVbRbZhFMsGySHDDcRd")); // Tablet en emulador: gonzalo
			var juana = crypto.GeneratePair(Base58.Decode("14V8igLr1ftDVGtgwrCUCAacdz43mzu1L2ZeumrEEkD1")); // Tablet en emulador: juana
			var alicia = crypto.GeneratePair(Base58.Decode("13MKf75enRiqyHnsiZAbz7pi94qUpDaGk29NaNFhkezZ")); // Tablet en emulador: alicia

			var tabletNeloNexus7 = Base58.Decode("NP9ajvEU6bWzN3CMdz1JCa2G34jVsi7ss7tzJCuJK52fbmXRzgky811Upko8tEjt4eSBaCsWKN7DwYYEdYLVSkxp");

			var publisher = args.Length == 0 ? new ConsoleAdapter()
				: args[0] == "web" ? new WebApiAdapter(args[1])
				: args[0] == "file" ? (IPublisher) new FileAdapter()
				: throw new ArgumentException("El parámetro args[0] no es válido");

			var factory = new Factory(crypto, publisher);

			factory.Load();

			var cryptoVoteId = await factory.Community("Crypto Vote", nelo);
			Task.WaitAll(new Task[]
			{
				factory.Member(cryptoVoteId, "Nelo", nelo.PublicKey, nelo),
				factory.Member(cryptoVoteId, "Martin", martin.PublicKey, nelo),
				factory.Member(cryptoVoteId, "Silvio", silvio.PublicKey, nelo),
				factory.Member(cryptoVoteId, "Neluz", tabletNeloNexus7, nelo)
			});

			var tasks = new List<Task>();
			tasks.Add(factory.Issue(cryptoVoteId, "El nodo debe correr en un container de Docker", new[]
			{
				new Choice {Id = Guid.NewGuid(), Color = 0x673ab7, Text = "Si"},
				new Choice {Id = Guid.NewGuid(), Color = 0xe91e63, Text = "No"},
			}, IssueType.DirectVote, nelo));
			tasks.Add(factory.Issue(cryptoVoteId, "Método de Encriptación para la Firma", new[]
			{
				new Choice {Id = Guid.NewGuid(), Color = 0x673ab7, Text = "secp256k1"},
				new Choice {Id = Guid.NewGuid(), Color = 0xe91e63, Text = "RSA"},
				new Choice {Id = Guid.NewGuid(), Color = 0xff5722, Text = "YAK"},
				new Choice {Id = Guid.NewGuid(), Color = 0x4caf50, Text = "ElGamal"},
			}, IssueType.DirectVote, nelo));
			tasks.Add(factory.Issue(cryptoVoteId, "¿Cómo se dan de alta las Organizaciones?", new[]
			{
				new Choice {Id = Guid.NewGuid(), Color = 0x673ab7, Text = "Cualquiera puede registrar una organización"},
				new Choice {Id = Guid.NewGuid(), Color = 0xe91e63, Text = "Solo el dueño de la Blockchain puede"},
				new Choice {Id = Guid.NewGuid(), Color = 0xff5722, Text = "Cualquier otra Organizaciones"},
				new Choice {Id = Guid.NewGuid(), Color = 0x4caf50, Text = "Cualquier que pague"},
			}, IssueType.DirectVote, nelo));
			tasks.Add(factory.Issue(cryptoVoteId, "¿Quién se dan de alta los temas a votar?", new[]
			{
				new Choice {Id = Guid.NewGuid(), Color = 0x673ab7, Text = "Cualquier miembro de una organización"},
				new Choice {Id = Guid.NewGuid(), Color = 0xe91e63, Text = "Solo el dueño de la organización"},
				new Choice {Id = Guid.NewGuid(), Color = 0x4caf50, Text = "Cualquier que pague"},
			}, IssueType.DirectVote, nelo));
			Task.WaitAll(tasks.ToArray());

			var eantId = await factory.Community("EANT", martin);
			Task.WaitAll(new Task[]
			{
				factory.Member(eantId, "Martin", martin.PublicKey, martin),
				factory.Member(eantId, "Gustavo", juana.PublicKey, martin),
				factory.Member(eantId, "Silvio", silvio.PublicKey, martin),
				factory.Member(eantId, "Romina", romina.PublicKey, martin),
			});

			await factory.Issue(eantId, "Nuevo Curso 2019 Q1", new[]
			{
				new Choice {Id = Guid.NewGuid(), Color = 0x673ab7, Text = "Blockchain"},
				new Choice {Id = Guid.NewGuid(), Color = 0xe91e63, Text = "Bitcoin"},
				new Choice {Id = Guid.NewGuid(), Color = 0xff5722, Text = "Smart Contract"},
				new Choice {Id = Guid.NewGuid(), Color = 0x4caf50, Text = "Arduino Intermedio"},
			}, IssueType.DirectVote, martin);

			/* ELECCIONES NACIONALES */
			var cne = crypto.GeneratePair();
			KeysPair apoderadoFrenteDespertar = crypto.GeneratePair(),
				apoderadorFrenteDeTodos = crypto.GeneratePair(),
				apoderadoFIT = crypto.GeneratePair(),
				apoderadoJuntosPorElCambio = crypto.GeneratePair(),
				apoderadoConsensoFederal = crypto.GeneratePair();

			Guid espertChoideId = Guid.NewGuid(),
				macriChoiceId = Guid.NewGuid(),
				fernandezChoiceId = Guid.NewGuid(),
				lavagnaChoiceId = Guid.NewGuid(),
				delcanoChoiceId = Guid.NewGuid();

			var argentinaId = await factory.Community("Argentina", cne);
			var eleccionesNacionalesId = await factory.Issue(argentinaId, "Elecciones Nacionales 2019", new[]
			{
				new Choice {Id = espertChoideId, Color = 0xf05c15, Text = "Espert, José Luis", GuardianAddress = apoderadoFrenteDespertar.PublicKey},
				new Choice {Id = macriChoiceId, Color = 0xffd204, Text = "Macri, Mauricio", GuardianAddress = apoderadoJuntosPorElCambio.PublicKey},
				new Choice {Id = fernandezChoiceId, Color = 0x59b6eb, Text = "Fernandez, Alberto", GuardianAddress = apoderadorFrenteDeTodos.PublicKey},
				new Choice {Id = lavagnaChoiceId, Color = 0xa900fd, Text = "Lavagna, Roberto", GuardianAddress = apoderadoConsensoFederal.PublicKey},
				new Choice {Id = delcanoChoiceId, Color = 0xfe0500, Text = "Del Caño, Nicolás", GuardianAddress = apoderadoFIT.PublicKey}
			}, IssueType.Recount, cne);

			Task.WaitAll(new[]
			{
				factory.Urn(eleccionesNacionalesId, "5468", new[] {nelo.PublicKey}, cne),
				factory.Urn(eleccionesNacionalesId, "1234", new[] {tabletNeloNexus7}, cne),

				factory.Fiscal(eleccionesNacionalesId, espertChoideId, romina.PublicKey, apoderadoFrenteDespertar),
				factory.Fiscal(eleccionesNacionalesId, fernandezChoiceId, alicia.PublicKey, apoderadorFrenteDeTodos),
				factory.Fiscal(eleccionesNacionalesId, delcanoChoiceId, martin.PublicKey, apoderadoFIT),
				factory.Fiscal(eleccionesNacionalesId, macriChoiceId, juana.PublicKey, apoderadoJuntosPorElCambio),
				factory.Fiscal(eleccionesNacionalesId, lavagnaChoiceId, juana.PublicKey, apoderadoConsensoFederal),
			});

			Console.WriteLine($"nelo: {Base58.Encode(nelo.PublicKey)}");
			Console.WriteLine($"martin: {Base58.Encode(martin.PublicKey)}");
			Console.WriteLine($"silvio: {Base58.Encode(silvio.PublicKey)}");
			Console.WriteLine($"romina: {Base58.Encode(romina.PublicKey)}");
			Console.WriteLine($"juana: {Base58.Encode(juana.PublicKey)}");
			Console.WriteLine($"alicia: {Base58.Encode(alicia.PublicKey)}");
		}
	}
}