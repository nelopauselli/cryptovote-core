using System;
using System.Linq;
using Domain;
using Domain.Crypto;
using Domain.Elections;
using NUnit.Framework;

namespace Tests
{
	public class Elecciones_nacionales_tests
	{
		private Blockchain blockchain;
		private KeysPair miner;
		private KeysPair camaraElectoral;
		private KeysPair apoderadoFrenteDespertar, apoderadoJuntosPorElCambio, apoderadorFrenteDeTodos, apoderadoConsensoFederal, apoderadoFIT;

		private KeysPair julio, albin;
		private KeysPair nelo, ana;
		private KeysPair sofia, maria, lucia, martina, catalina, elena, emilia, valentina, paula, zoe;
		private KeysPair santiago, mateo, juan, matias, nicolas, benjamin, pedro, tomas, thiago, santino;

		private Signer signer;

		private Guid communityId, eleccionId, mesa0001Id, mesa5468Id;
		private Guid espertChoideId, macriChoiceId, fernandezChoiceId, lavagnaChoiceId, delcanoChoiceId;

		[OneTimeSetUp]
		public void InitKeys()
		{
			miner = CryptoService.Instance.GeneratePair();
			camaraElectoral = CryptoService.Instance.GeneratePair();

			apoderadoFrenteDespertar = CryptoService.Instance.GeneratePair();
			apoderadorFrenteDeTodos = CryptoService.Instance.GeneratePair();
			apoderadoFIT = CryptoService.Instance.GeneratePair();
			apoderadoJuntosPorElCambio = CryptoService.Instance.GeneratePair();
			apoderadoConsensoFederal = CryptoService.Instance.GeneratePair();

			julio = CryptoService.Instance.GeneratePair();
			albin = CryptoService.Instance.GeneratePair();
			nelo = CryptoService.Instance.GeneratePair();
			ana = CryptoService.Instance.GeneratePair();

			sofia = CryptoService.Instance.GeneratePair();
			maria = CryptoService.Instance.GeneratePair();
			lucia = CryptoService.Instance.GeneratePair();
			martina = CryptoService.Instance.GeneratePair();
			catalina = CryptoService.Instance.GeneratePair();
			elena = CryptoService.Instance.GeneratePair();
			emilia = CryptoService.Instance.GeneratePair();
			valentina = CryptoService.Instance.GeneratePair();
			paula = CryptoService.Instance.GeneratePair();
			zoe = CryptoService.Instance.GeneratePair();

			santiago = CryptoService.Instance.GeneratePair();
			mateo = CryptoService.Instance.GeneratePair();
			juan = CryptoService.Instance.GeneratePair();
			matias = CryptoService.Instance.GeneratePair();
			nicolas = CryptoService.Instance.GeneratePair();
			benjamin = CryptoService.Instance.GeneratePair();
			pedro = CryptoService.Instance.GeneratePair();
			tomas = CryptoService.Instance.GeneratePair();
			thiago = CryptoService.Instance.GeneratePair();
			santino = CryptoService.Instance.GeneratePair();

			signer = new Signer(CryptoService.Instance);

			communityId = Guid.NewGuid();
			eleccionId = Guid.NewGuid();
			mesa0001Id = Guid.NewGuid();
			mesa5468Id = Guid.NewGuid();

			espertChoideId = Guid.NewGuid();
			macriChoiceId = Guid.NewGuid();
			fernandezChoiceId = Guid.NewGuid();
			lavagnaChoiceId = Guid.NewGuid();
			delcanoChoiceId = Guid.NewGuid();
		}

		[SetUp]
		public void Init()
		{
			blockchain = new Blockchain(new Miner(miner.PublicKey), new BlockBuilder(), 1);
			blockchain.LoadGenesisBlock("genesis.block");
		}

		[Test]
		public void RegistrandoComunidad()
		{
			var community = new Community
			{
				Id = communityId,
				Name = "Electores Argentina",
				Address = camaraElectoral.PublicKey
			};
			signer.Sign(community, camaraElectoral);

			var block = blockchain.MineNextBlock(new BlockItem[] {community});
			Assert.IsNotNull(block);
			Assert.AreEqual(1, block.Communities.Count);
		}

		[Test]
		public void RegistrandoEleccionPresidencial()
		{
			RegistrandoComunidad();

			var eleccion = new Question
			{
				Id = eleccionId,
				Name = "Elecciones Nacionales a Presidente",
				CommunityId = communityId,
				Type = QuestionType.Recount,
				Choices = new[]
				{
					new Choice {Id = espertChoideId, Text = "Espert, José Luis", GuardianAddress = apoderadoFrenteDespertar.PublicKey},
					new Choice {Id = macriChoiceId, Text = "Macri, Mauricio", GuardianAddress = apoderadoJuntosPorElCambio.PublicKey},
					new Choice {Id = fernandezChoiceId, Text = "Fernandez, Alberto", GuardianAddress = apoderadorFrenteDeTodos.PublicKey},
					new Choice {Id = lavagnaChoiceId, Text = "Lavagna, Roberto", GuardianAddress = apoderadoConsensoFederal.PublicKey},
					new Choice {Id = delcanoChoiceId, Text = "Del Caño, Nicolás", GuardianAddress = apoderadoFIT.PublicKey}
				}
			};

			signer.Sign(eleccion, camaraElectoral);

			Assert.IsTrue(eleccion.IsValid(blockchain.Trunk.ToArray()));

			var block = blockchain.MineNextBlock(new BlockItem[] { eleccion });
			Assert.IsNotNull(block);
			Assert.AreEqual(1, block.Questions.Count);
		}

		[Test]
		public void RegistrandoUrnas()
		{
			RegistrandoEleccionPresidencial();

			var urna0001 = new Urn {Id = mesa0001Id, QuestionId = eleccionId, Name = "0001", Authorities = new[] {julio.PublicKey, albin.PublicKey}};
			signer.Sign(urna0001, camaraElectoral);

			var urna5468 = new Urn {Id = mesa5468Id, QuestionId = eleccionId, Name = "5468", Authorities = new[] {nelo.PublicKey, ana.PublicKey}};
			signer.Sign(urna5468, camaraElectoral);

			var block = blockchain.MineNextBlock(new BlockItem[] {urna0001, urna5468});
			Assert.IsNotNull(block);
			Assert.AreEqual(2, block.Urns.Count);
		}

		[Test]
		public void RegistrandoFiscales()
		{
			RegistrandoUrnas();

			var fiscalFrenteDesperar0001 = new Fiscal {QuestionId = eleccionId, ChoiceId = espertChoideId, Address = sofia.PublicKey};
			signer.Sign(fiscalFrenteDesperar0001, apoderadoFrenteDespertar);
			var fiscalFrenteDesperar5468 = new Fiscal {QuestionId = eleccionId, ChoiceId = espertChoideId, Address = santiago.PublicKey};
			signer.Sign(fiscalFrenteDesperar5468, apoderadoFrenteDespertar);

			var fiscalJuntosPorElCambio0001 = new Fiscal {QuestionId = eleccionId, ChoiceId = macriChoiceId, Address = maria.PublicKey};
			signer.Sign(fiscalJuntosPorElCambio0001, apoderadoJuntosPorElCambio);
			var fiscalJuntosPorElCambio5468 = new Fiscal {QuestionId = eleccionId, ChoiceId = macriChoiceId, Address = mateo.PublicKey};
			signer.Sign(fiscalJuntosPorElCambio5468, apoderadoJuntosPorElCambio);

			var fiscalFrenteDeTodos0001 = new Fiscal {QuestionId = eleccionId, ChoiceId = fernandezChoiceId, Address = lucia.PublicKey};
			signer.Sign(fiscalFrenteDeTodos0001, apoderadorFrenteDeTodos);
			var fiscalFrenteDeTodos5468 = new Fiscal {QuestionId = eleccionId, ChoiceId = fernandezChoiceId, Address = juan.PublicKey};
			signer.Sign(fiscalFrenteDeTodos5468, apoderadorFrenteDeTodos);

			var block = blockchain.MineNextBlock(new BlockItem[]
			{
				fiscalFrenteDesperar0001, fiscalFrenteDesperar5468,
				fiscalJuntosPorElCambio0001, fiscalJuntosPorElCambio5468,
				fiscalFrenteDeTodos0001, fiscalFrenteDeTodos5468
			});
			Assert.IsNotNull(block);
			Assert.AreEqual(6, block.Fiscals.Count);
		}

		[Test]
		public void RegistrandoRecuentos()
		{
			RegistrandoFiscales();

			var recuento0001 = new Recount
			{
				UrnId = mesa0001Id,
				Results = new[] {new ChoiceRecount {ChoiceId = espertChoideId, Votes = 10}, new ChoiceRecount {ChoiceId = macriChoiceId, Votes = 80}, new ChoiceRecount {ChoiceId = fernandezChoiceId, Votes = 117}, new ChoiceRecount {ChoiceId = lavagnaChoiceId, Votes = 17}, new ChoiceRecount {ChoiceId = delcanoChoiceId, Votes = 12}}
			};
			signer.Sign(recuento0001, julio);

			var recuento5468 = new Recount
			{
				UrnId = mesa5468Id,
				Results = new[] { new ChoiceRecount { ChoiceId = espertChoideId, Votes = 12 }, new ChoiceRecount { ChoiceId = macriChoiceId, Votes = 131 }, new ChoiceRecount { ChoiceId = fernandezChoiceId, Votes = 71}, new ChoiceRecount { ChoiceId = lavagnaChoiceId, Votes = 29}, new ChoiceRecount { ChoiceId = delcanoChoiceId, Votes = 9 } }
			};
			signer.Sign(recuento5468, nelo);

			var block = blockchain.MineNextBlock(new BlockItem[]
			{
				recuento0001, recuento5468
			});

			Assert.IsNotNull(block);
			Assert.AreEqual(2, block.Recounts.Count);
		}

		[Test]
		public void FiscalIntentandoFirmarRecuento()
		{
			RegistrandoUrnas();

			var recuento0001 = new Recount
			{
				UrnId = mesa0001Id,
				Results = new[] { new ChoiceRecount { ChoiceId = espertChoideId, Votes = 10 }, new ChoiceRecount { ChoiceId = macriChoiceId, Votes = 80 }, new ChoiceRecount { ChoiceId = fernandezChoiceId, Votes = 117 }, new ChoiceRecount { ChoiceId = lavagnaChoiceId, Votes = 17 }, new ChoiceRecount { ChoiceId = delcanoChoiceId, Votes = 12 } }
			};
			signer.Sign(recuento0001, lucia);

			var recuento5468 = new Recount
			{
				UrnId = mesa5468Id,
				Results = new[] { new ChoiceRecount { ChoiceId = espertChoideId, Votes = 12 }, new ChoiceRecount { ChoiceId = macriChoiceId, Votes = 131 }, new ChoiceRecount { ChoiceId = fernandezChoiceId, Votes = 71 }, new ChoiceRecount { ChoiceId = lavagnaChoiceId, Votes = 29 }, new ChoiceRecount { ChoiceId = delcanoChoiceId, Votes = 9 } }
			};
			signer.Sign(recuento5468, nelo);

			var block = blockchain.MineNextBlock(new BlockItem[]
			{
				recuento0001, recuento5468
			});

			Assert.IsNotNull(block);
			Assert.AreEqual(1, block.Recounts.Count);
		}

		[Test]
		public void DesconocidoIntentandoFirmarRecuento()
		{
			RegistrandoUrnas();

			var recuento0001 = new Recount
			{
				UrnId = mesa0001Id,
				Results = new[] { new ChoiceRecount { ChoiceId = espertChoideId, Votes = 10 }, new ChoiceRecount { ChoiceId = macriChoiceId, Votes = 80 }, new ChoiceRecount { ChoiceId = fernandezChoiceId, Votes = 117 }, new ChoiceRecount { ChoiceId = lavagnaChoiceId, Votes = 17 }, new ChoiceRecount { ChoiceId = delcanoChoiceId, Votes = 12 } }
			};
			signer.Sign(recuento0001, zoe);

			var recuento5468 = new Recount
			{
				UrnId = mesa5468Id,
				Results = new[] { new ChoiceRecount { ChoiceId = espertChoideId, Votes = 12 }, new ChoiceRecount { ChoiceId = macriChoiceId, Votes = 131 }, new ChoiceRecount { ChoiceId = fernandezChoiceId, Votes = 71 }, new ChoiceRecount { ChoiceId = lavagnaChoiceId, Votes = 29 }, new ChoiceRecount { ChoiceId = delcanoChoiceId, Votes = 9 } }
			};
			signer.Sign(recuento5468, santino);

			var block = blockchain.MineNextBlock(new BlockItem[]
			{
				recuento0001, recuento5468
			});

			Assert.IsNull(block);
		}
		
		[Test]
		public void RecuentoFirmadoPorSuplente()
		{
			RegistrandoUrnas();

			var recuento0001 = new Recount
			{
				UrnId = mesa0001Id,
				Results = new[] { new ChoiceRecount { ChoiceId = espertChoideId, Votes = 10 }, new ChoiceRecount { ChoiceId = macriChoiceId, Votes = 80 }, new ChoiceRecount { ChoiceId = fernandezChoiceId, Votes = 117 }, new ChoiceRecount { ChoiceId = lavagnaChoiceId, Votes = 17 }, new ChoiceRecount { ChoiceId = delcanoChoiceId, Votes = 12 } }
			};
			signer.Sign(recuento0001, albin);

			var recuento5468 = new Recount
			{
				UrnId = mesa5468Id,
				Results = new[] { new ChoiceRecount { ChoiceId = espertChoideId, Votes = 12 }, new ChoiceRecount { ChoiceId = macriChoiceId, Votes = 131 }, new ChoiceRecount { ChoiceId = fernandezChoiceId, Votes = 71 }, new ChoiceRecount { ChoiceId = lavagnaChoiceId, Votes = 29 }, new ChoiceRecount { ChoiceId = delcanoChoiceId, Votes = 9 } }
			};
			signer.Sign(recuento5468, ana);

			var block = blockchain.MineNextBlock(new BlockItem[]
			{
				recuento0001, recuento5468
			});

			Assert.IsNotNull(block);
			Assert.AreEqual(2, block.Recounts.Count);
		}

		[Test]
		public void ConvalidandoRecuentosPorSuplentes()
		{
			RegistrandoRecuentos();

			var recuento0001 = blockchain.GetRecount(mesa0001Id);
			var recognition0001 = new Recognition {Id = Guid.NewGuid(), UrnId = mesa0001Id, Content = recuento0001.GetData()};
			signer.Sign(recognition0001, albin);

			var recuento5468 = blockchain.GetRecount(mesa5468Id);
			var recognition5468 = new Recognition {Id = Guid.NewGuid(), UrnId = mesa5468Id, Content = recuento5468.GetData()};
			signer.Sign(recognition5468, ana);

			var block = blockchain.MineNextBlock(new BlockItem[]
			{
				recognition0001, recognition5468
			});

			Assert.IsNotNull(block);
			Assert.AreEqual(2, block.Recognitions.Count);
		}

		[Test]
		public void Recuento()
		{
			RegistrandoRecuentos();

			Assert.AreEqual(10 + 12, blockchain.GetRecountFor(eleccionId, espertChoideId));
			Assert.AreEqual(80 + 131, blockchain.GetRecountFor(eleccionId, macriChoiceId));
			Assert.AreEqual(117 + 71, blockchain.GetRecountFor(eleccionId, fernandezChoiceId));
			Assert.AreEqual(17 + 29, blockchain.GetRecountFor(eleccionId, lavagnaChoiceId));
			Assert.AreEqual(12 + 9, blockchain.GetRecountFor(eleccionId, delcanoChoiceId));
		}

		[Test]
		public void ConvalidandoRecuentosPorFiscales()
		{
			ConvalidandoRecuentosPorSuplentes();

			var recuento0001 = blockchain.GetRecount(mesa0001Id);
			var recognition0001FrenteDeTodos = new Recognition { Id = Guid.NewGuid(), UrnId = mesa0001Id, Content = recuento0001.GetData() };
			signer.Sign(recognition0001FrenteDeTodos, lucia);

			var recuento5468 = blockchain.GetRecount(mesa5468Id);
			var recognition5468JuntosPorElCambio = new Recognition { Id = Guid.NewGuid(), UrnId = mesa5468Id, Content = recuento5468.GetData() };
			signer.Sign(recognition5468JuntosPorElCambio, mateo);
			var recognition5468FrenteDeTodos = new Recognition { Id = Guid.NewGuid(), UrnId = mesa5468Id, Content = recuento5468.GetData() };
			signer.Sign(recognition5468FrenteDeTodos, juan);
			var block = blockchain.MineNextBlock(new BlockItem[]
			{
				recognition0001FrenteDeTodos, recognition5468JuntosPorElCambio, recognition5468FrenteDeTodos
			});

			Assert.IsNotNull(block);
			Assert.AreEqual(3, block.Recognitions.Count);
		}

		[Test]
		public void ConvalidandoRecuentosPorFiscalesDelMismoPartido()
		{
			ConvalidandoRecuentosPorSuplentes();

			var recuento0001 = blockchain.GetRecount(mesa0001Id);
			var recognition0001FrenteDeTodos = new Recognition { Id = Guid.NewGuid(), UrnId = mesa0001Id, Content = recuento0001.GetData() };
			signer.Sign(recognition0001FrenteDeTodos, lucia);

			var recuento5468 = blockchain.GetRecount(mesa5468Id);
			var recognition5468JuntosPorElCambio = new Recognition { Id = Guid.NewGuid(), UrnId = mesa5468Id, Content = recuento5468.GetData() };
			signer.Sign(recognition5468JuntosPorElCambio, mateo);
			var recognition5468FrenteDeTodos1 = new Recognition { Id = Guid.NewGuid(), UrnId = mesa5468Id, Content = recuento5468.GetData() };
			signer.Sign(recognition5468FrenteDeTodos1, juan);

			// Este fiscal representa a la misma opción que el anterior,
			// es válido pero solo será contabilizado como una confirmación
			var recognition5468FrenteDeTodos2 = new Recognition { Id = Guid.NewGuid(), UrnId = mesa5468Id, Content = recuento5468.GetData() };
			signer.Sign(recognition5468FrenteDeTodos2, lucia);

			var block = blockchain.MineNextBlock(new BlockItem[]
			{
				recognition0001FrenteDeTodos, recognition5468JuntosPorElCambio, recognition5468FrenteDeTodos1, recognition5468FrenteDeTodos2
			});

			Assert.IsNotNull(block);
			Assert.AreEqual(4, block.Recognitions.Count);
		}

		[Test]
		public void Confirmaciones()
		{
			ConvalidandoRecuentosPorFiscales();

			Assert.AreEqual(2, blockchain.CountRecognitions(mesa0001Id));
			Assert.AreEqual(3, blockchain.CountRecognitions(mesa5468Id));
		}

		[Test]
		public void ConfirmacionesDeFiscalesDelMismoPartido()
		{
			ConvalidandoRecuentosPorFiscalesDelMismoPartido();

			Assert.AreEqual(2, blockchain.CountRecognitions(mesa0001Id));
			Assert.AreEqual(3, blockchain.CountRecognitions(mesa5468Id));
		}

		[Test]
		public void Totalizando_resultados()
		{
			ConvalidandoRecuentosPorFiscales();

			var result = blockchain.GetResult(eleccionId);

			Assert.AreEqual(eleccionId, result.QuestionId);
			Assert.AreEqual(espertChoideId, result.Choices[0].ChoiceId);
			Assert.AreEqual(10+12, result.Choices[0].Votes);
			Assert.AreEqual(macriChoiceId, result.Choices[1].ChoiceId);
			Assert.AreEqual(80 + 131, result.Choices[1].Votes);
			Assert.AreEqual(fernandezChoiceId, result.Choices[2].ChoiceId);
			Assert.AreEqual(117 + 71, result.Choices[2].Votes);
			Assert.AreEqual(lavagnaChoiceId, result.Choices[3].ChoiceId);
			Assert.AreEqual(17 + 29, result.Choices[3].Votes);
			Assert.AreEqual(delcanoChoiceId, result.Choices[4].ChoiceId);
			Assert.AreEqual(12 + 9, result.Choices[4].Votes);
		}
	}
}