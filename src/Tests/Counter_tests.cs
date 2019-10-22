using System;
using System.Linq;
using Domain;
using Domain.Crypto;
using Domain.Scrutiny;
using NUnit.Framework;

namespace Tests
{
	public class Counter_tests
	{
		private Blockchain blockchain;
		private KeysPair miner, root, pedro;
		private KeysPair alicia, roberto, camila;
		private Signer signer;
		
		private Guid juanId, joseId;

		[OneTimeSetUp]
		public void InitKeys()
		{
			miner = CryptoService.Instance.GeneratePair();
			root = CryptoService.Instance.GeneratePair();
			pedro = CryptoService.Instance.GeneratePair();
			alicia = CryptoService.Instance.GeneratePair();
			roberto = CryptoService.Instance.GeneratePair();
			camila = CryptoService.Instance.GeneratePair();

			signer = new Signer(CryptoService.Instance);
		}

		[SetUp]
		public void Init()
		{
			blockchain = new Blockchain(new Miner(miner.PublicKey), new BlockBuilder(), 2);
			blockchain.LoadGenesisBlock("genesis.block");

			var community = new Community
			{
				Id = Guid.NewGuid(),
				Address = pedro.PublicKey,
				Name = "Club Pepito",
				CreateAt = DateTimeOffset.UtcNow
			};
			signer.Sign(community, root);

			blockchain.MineNextBlock(new BlockItem[] { community });

			juanId = Guid.NewGuid();
			joseId = Guid.NewGuid();
			var issues = new BlockItem[]
			{
				new Issue { CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Elección de Presidente",
					Choices = new [] {new Choice{ Id = juanId, Text = "Juan"}, new Choice { Id = joseId, Text = "Jose" } }},
			};
			signer.Sign(issues[0], pedro);
			blockchain.MineNextBlock(issues);

			var members = new BlockItem[]
			{
				new Member {CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Alicia", Address = alicia.PublicKey},
				new Member {CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Ruberto", Address = roberto.PublicKey},
				new Member {CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Camila", Address = camila.PublicKey}
			};

			signer.Sign(members[0], pedro);
			signer.Sign(members[1], pedro);
			signer.Sign(members[2], pedro);

			blockchain.MineNextBlock(members);
		}

		[Test]
		public void Count_3_votes()
		{
			var issueId = Guid.NewGuid();

			var pendings = new BlockItem[]
			{
				new Vote {IssueId = issueId, ChoiceId = juanId},
				new Vote {IssueId = issueId, ChoiceId = joseId},
				new Vote {IssueId = issueId, ChoiceId = juanId},
			};

			signer.Sign(pendings[0], alicia);
			signer.Sign(pendings[1], roberto);
			signer.Sign(pendings[2], camila);

			blockchain.MineNextBlock(pendings);

			var counter = new Counter(blockchain);

			Assert.AreEqual(2, counter.TotalFor(juanId));
			Assert.AreEqual(1, counter.TotalFor(joseId));
		}

		[Test]
		public void Count_votes_with_repeated()
		{
			var issueId = Guid.NewGuid();

			var pendings = new BlockItem[]
			{
				new Vote {IssueId = issueId, ChoiceId = juanId},
				new Vote {IssueId = issueId, ChoiceId = joseId},
				new Vote {IssueId = issueId, ChoiceId = juanId},
			};

			signer.Sign(pendings[0], alicia);
			signer.Sign(pendings[1], roberto);
			signer.Sign(pendings[2], alicia);

			Assert.IsFalse(pendings[0].PublicKey.SequenceEqual(pendings[1].PublicKey));
			Assert.IsTrue(pendings[0].PublicKey.SequenceEqual(pendings[2].PublicKey));

			blockchain.MineNextBlock(pendings);
			
			var counter = new Counter(blockchain);

			Assert.AreEqual(1, counter.TotalFor(juanId));
			Assert.AreEqual(1, counter.TotalFor(joseId));
		}
	}
}