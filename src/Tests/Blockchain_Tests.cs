using System;
using System.Linq;
using System.Text;
using Domain;
using Domain.Crypto;
using Domain.Queries;
using Domain.Elections;
using NUnit.Framework;

namespace Tests
{
	public class Blockchain_tests
	{
		private Blockchain blockchain;
		private KeysPair miner, root, pedro;
		private KeysPair alicia, roberto, camila;
		private KeysPair ghost;
		private Signer signer;

		[OneTimeSetUp]
		public void InitKeys()
		{
			miner = CryptoService.Instance.GeneratePair();
			root = CryptoService.Instance.GeneratePair();
			pedro = CryptoService.Instance.GeneratePair();
			alicia = CryptoService.Instance.GeneratePair();
			roberto = CryptoService.Instance.GeneratePair();
			camila = CryptoService.Instance.GeneratePair();
			ghost = CryptoService.Instance.GeneratePair();

			signer = new Signer(CryptoService.Instance);
		}

		[SetUp]
		public void Init()
		{
			blockchain = new Blockchain(new Miner(miner.PublicKey), new BlockBuilder(), 1);
			blockchain.LoadGenesisBlock("genesis.block");
		}

		[Test]
		public void Genesis()
		{
			Assert.AreEqual(1, blockchain.Trunk.Count());

			var block = blockchain.GetBlock(0);
			Assert.IsTrue(block.PreviousHash.SequenceEqual(Array.Empty<byte>()));
			Assert.IsFalse(block.Hash.SequenceEqual(Array.Empty<byte>()));

			var buffer = new byte[2];
			Buffer.BlockCopy(block.Hash, 0, buffer, 0, 2);
			Assert.IsTrue(buffer.SequenceEqual(new byte[] {0, 0}));
		}

		[Test]
		public void Community_register()
		{
			var clubPepito = new Community
			{
				Id = Guid.NewGuid(),
				Address = pedro.PublicKey,
				Name = "Club Pepito",
				CreateAt = DateTimeOffset.UtcNow
			};
			signer.Sign(clubPepito, root);

			var pendings = new BlockItem[] {clubPepito};
			var block = blockchain.MineNextBlock(pendings);
			Assert.IsNotNull(block);
			Assert.AreEqual(1, block.Communities.Count);
			Assert.AreEqual(2, blockchain.Trunk.Count());

			var previousBlock = blockchain.GetBlock(0);
			Assert.IsTrue(block.PreviousHash.SequenceEqual(previousBlock.Hash));
			Assert.IsTrue(block.IsValid());
		}

		[Test]
		public void Community_list()
		{
			Community_register();

			var communities = blockchain.Trunk.SelectMany(c => c.Communities);
			Assert.AreEqual(1, communities.Count());
		}

		[Test]
		public void Issue_register()
		{
			//Community_register();
			var clubPepito = new Community
			{
				Id = Guid.NewGuid(),
				Address = pedro.PublicKey,
				Name = "Club Pepito",
				CreateAt = DateTimeOffset.UtcNow
			};
			signer.Sign(clubPepito, root);

			var block1 = blockchain.MineNextBlock(new BlockItem[] {clubPepito});
			Assert.AreEqual(1, block1.Communities.Count);

			var community = blockchain.Trunk.SelectMany(c => c.Communities).FirstOrDefault();
			Assert.IsNotNull(community);

			var pendings = new BlockItem[]
			{
				new Issue
				{
					CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Elección de Presidente",
					Choices = new[] {new Choice {Id = Guid.NewGuid(), Text = "Juan"}, new Choice {Id = Guid.NewGuid(), Text = "Jose"}}
				},
				new Issue
				{
					CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Color de la bandera",
					Choices = new[] {new Choice {Id = Guid.NewGuid(), Text = "Rojo y Azul"}, new Choice {Id = Guid.NewGuid(), Text = "Rojo y Verde"}, new Choice {Id = Guid.NewGuid(), Text = "Amarillo y Verde"}, new Choice {Id = Guid.NewGuid(), Text = "Rojo, Amarillo y Verde"}}
				}
			};

			signer.Sign(pendings[0], pedro);
			signer.Sign(pendings[1], pedro);

			var block = blockchain.MineNextBlock(pendings);
			Assert.IsNotNull(block);
			Assert.AreEqual(2, block.Issues.Count);
			Assert.AreEqual(3, blockchain.Trunk.Count());

			var previousBlock = blockchain.GetBlock(1);
			Assert.IsTrue(block.PreviousHash.SequenceEqual(previousBlock.Hash));
			Assert.IsTrue(block.IsValid());
		}

		[Test]
		public void Issue_list()
		{
			Issue_register();

			var issues = blockchain.Trunk.SelectMany(c=>c.Issues);
			Assert.AreEqual(2, issues.Count());
		}

		[Test]
		public void Members_Register()
		{
			Issue_register();
			var community = blockchain.Trunk.SelectMany(c => c.Communities).First();

			var pendings = new BlockItem[]
			{
				new Member
				{
					CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Alicia",
					Address = alicia.PublicKey
				},
				new Member
				{
					CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Ruberto",
					Address = roberto.PublicKey
				},
				new Member
				{
					CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Camila",
					Address = camila.PublicKey
				}
			};

			signer.Sign(pendings[0], pedro);
			signer.Sign(pendings[1], pedro);
			signer.Sign(pendings[2], pedro);

			var block = blockchain.MineNextBlock(pendings);
			Assert.AreEqual(3, block.Members.Count);
			Assert.AreEqual(4, blockchain.Trunk.Count());

			Assert.IsNotNull(block);

			var previousBlock = blockchain.GetBlock(2);
			Assert.IsTrue(block.PreviousHash.SequenceEqual(previousBlock.Hash));
			Assert.IsTrue(block.IsValid());
		}

		[Test]
		public void Vote_Add()
		{
			Members_Register();
			var issue = blockchain.Trunk.SelectMany(c => c.Issues).First();

			var pendings = new BlockItem[]
			{
				new Vote {IssueId = issue.Id, ChoiceId = issue.Choices[0].Id},
				new Vote {IssueId = issue.Id, ChoiceId = issue.Choices[1].Id},
				new Vote {IssueId = issue.Id, ChoiceId = issue.Choices[0].Id},
			};

			signer.Sign(pendings[0], alicia);
			signer.Sign(pendings[1], roberto);
			signer.Sign(pendings[2], camila);

			var block = blockchain.MineNextBlock(pendings);
			Assert.IsNotNull(block);
			Assert.AreEqual(3, block.Votes.Count);
			Assert.AreEqual(5, blockchain.Trunk.Count());

			var previousBlock = blockchain.GetBlock(3);
			Assert.IsTrue(block.PreviousHash.SequenceEqual(previousBlock.Hash));
			Assert.IsTrue(block.IsValid());
		}

		[Test]
		public void Vote_list()
		{
			Vote_Add();

			var votes = blockchain.Trunk.SelectMany(c => c.Votes);
			Assert.AreEqual(3, votes.Count());
		}

		[Test]
		public void Vote_duplicated()
		{
			Members_Register();
			var issue = blockchain.Trunk.SelectMany(c => c.Issues).First();

			var pendings1 = new BlockItem[]
			{
				new Vote {IssueId = issue.Id, ChoiceId = issue.Choices[0].Id},
				new Vote {IssueId = issue.Id, ChoiceId = issue.Choices[1].Id} 
			};

			signer.Sign(pendings1[0], alicia);
			signer.Sign(pendings1[1], roberto);

			blockchain.MineNextBlock(pendings1);

			var pendings2 = new BlockItem[]
			{
				new Vote {IssueId = issue.Id, ChoiceId = issue.Choices[1].Id}, // este voto está repetido
				new Vote {IssueId = issue.Id, ChoiceId = issue.Choices[0].Id}
			};
			signer.Sign(pendings2[0], roberto);
			signer.Sign(pendings2[1], camila);

			blockchain.MineNextBlock(pendings2);

			var query = new VotesQuery(blockchain);
			var result = query.Execute(issue.Id.ToString("n")).ToArray();

			Assert.AreEqual(3, result.Length);
		}

		[Test]
		public void Validate()
		{
			Vote_Add();
			Assert.IsTrue(blockchain.IsValid());
		}

		[Test]
		public void Ghost_signing_community()
		{
			var community = new Community {Id = Guid.NewGuid(), Name = "Club Fantasma", CreateAt = DateTimeOffset.UtcNow};
			signer.Sign(community, ghost);

			var previousLength = blockchain.Trunk.Count();

			var block = blockchain.MineNextBlock(new BlockItem[] {community});

			Assert.AreEqual(1, block.Communities.Count);
			Assert.AreEqual(previousLength + 1, blockchain.Trunk.Count());
		}

		[Test]
		public void Ghost_signing_issue()
		{
			Community_register();

			var community = blockchain.Trunk.SelectMany(c => c.Communities).First();

			var issue = new Issue
			{
				CommunityId = community.Id, Id = Guid.NewGuid(), Name = "Elección de Presidente",
				Choices = new[] {new Choice { Id = Guid.NewGuid(), Text = "Juan"}, new Choice { Id = Guid.NewGuid(), Text = "Jose"}}
			};

			signer.Sign(issue, ghost);

			var previousLength = blockchain.Trunk.Count();

			var block = blockchain.MineNextBlock(new BlockItem[] {issue});
			Assert.IsNull(block);

			Assert.AreEqual(previousLength, blockchain.Trunk.Count());
		}

		[Test]
		public void Ghost_signing_member()
		{
			Community_register();

			var community = blockchain.Trunk.SelectMany(c => c.Communities).First();

			var gasparin = new Member
			{
				CommunityId = community.Id,
				Id = Guid.NewGuid(),
				Name = "Gasparin",
			};

			signer.Sign(gasparin, ghost);

			var previousLength = blockchain.Trunk.Count();

			var block = blockchain.MineNextBlock(new BlockItem[] {gasparin});
			Assert.IsNull(block);
			Assert.AreEqual(previousLength, blockchain.Trunk.Count());
		}

		[Test]
		public void Ghost_signing_vote()
		{
			Members_Register();

			var issue = blockchain.Trunk.SelectMany(c => c.Issues).First();

			var vote = new Vote
			{
				IssueId = issue.Id,
				ChoiceId = issue.Choices[0].Id,
				Time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};

			signer.Sign(vote, ghost);

			var previousLength = blockchain.Trunk.Count();

			var block = blockchain.MineNextBlock(new BlockItem[] {vote});
			Assert.IsNull(block);

			Assert.AreEqual(previousLength, blockchain.Trunk.Count());
		}
	}
}