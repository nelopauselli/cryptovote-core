using System;
using System.Linq;
using CryptoVote.Controllers;
using Domain;
using Domain.Crypto;
using Domain.Elections;
using Domain.Utils;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace Tests
{
	public class Conversation_tests : TestWithServices
	{
		private string host = "127.0.0.1";
		private int port = 14001;

		private Blockchain blockchain;
		private KeysPair miner, root, nelo;
		private Signer signer;

		private Guid cryptoVoteId, questionId, neloId;
		private byte[] block1Hash;

		[OneTimeSetUp]
		public void InitKeys()
		{
			miner = CryptoService.Instance.GeneratePair();
			root = CryptoService.Instance.GeneratePair();
			nelo = CryptoService.Instance.GeneratePair();

			signer = new Signer(CryptoService.Instance);

			cryptoVoteId = Guid.NewGuid();
			questionId = Guid.NewGuid();
			neloId = Guid.NewGuid();
		}

		[SetUp]
		public void Init()
		{
			blockchain = new Blockchain(new Miner(miner.PublicKey), new BlockBuilder(), 2);
			blockchain.LoadGenesisBlock("genesis.block");

			var cryptoVote = new Community {Id = cryptoVoteId, Name = "Crypto Vote", Address = nelo.PublicKey};
			signer.Sign(cryptoVote, root);
			var copperadora = new Community {Id = Guid.NewGuid(), Name = "Cooperadora X"};
			signer.Sign(copperadora, root);

			var question = new Question {Id = questionId, CommunityId = cryptoVote.Id, Name = "¿el nodo debe poder ejecutarse en una Raspberry?", Choices = new[] {new Choice {Id = Guid.NewGuid(), Text = "SI"}, new Choice {Id = Guid.NewGuid(), Text = "NO"}}};
			signer.Sign(question, nelo);

			var member = new Member {Id = neloId, CommunityId = cryptoVote.Id, Name = "Nelo"};
			signer.Sign(member, nelo);

			blockchain.MineNextBlock(new BlockItem[] {cryptoVote, copperadora});
			var block1 = blockchain.MineNextBlock(new BlockItem[] {question, member});
			block1Hash = block1.Hash;
		}

		[Test]
		public void Get_last_block()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var controller = new ChainController(node.Object, new NullLogger<ChainController>());

			var block = controller.GetLast();

			Assert.IsNotNull(block);
			Assert.AreEqual(1, block.Questions.Count);
			Assert.AreEqual(1, block.Members.Count);
		}

		[Test]
		public void Get_block()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var controller = new ChainController(node.Object, new NullLogger<ChainController>());

			var response = controller.Get(Base58.Encode(block1Hash));
			Assert.AreEqual(200, response.StatusCode);

			var block = (Block) response.Value;
			Assert.IsNotNull(block);
			Assert.AreEqual(1, block.Questions.Count);
			Assert.AreEqual(1, block.Members.Count);
		}

		[Test]
		public void Communities_list()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var controller = new CommunityController(node.Object, new NullLogger<CommunityController>());

			var communities = controller.List();

			Assert.IsNotNull(communities);
			Assert.AreEqual(2, communities.Count());
		}

		[Test]
		public void Community_get()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var controller = new CommunityController(node.Object, new NullLogger<CommunityController>());

			var community = controller.Get(cryptoVoteId);

			Assert.IsNotNull(community);
			Assert.AreEqual("Crypto Vote", community.Name);
		}

		[Test]
		public void Questions_list()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var controller = new QuestionController(node.Object, new NullLogger<QuestionController>());

			var questions = controller.List(cryptoVoteId).ToArray();

			Assert.AreEqual(1, questions.Length);

			var question = questions[0];
			Assert.IsNotNull(question);
			Assert.AreEqual("¿el nodo debe poder ejecutarse en una Raspberry?", question.Name);
		}

		[Test]
		public void Question_get()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var controller = new QuestionController(node.Object, new NullLogger<QuestionController>());


			var question = controller.Get(cryptoVoteId, questionId);

			Assert.IsNotNull(question);
			Assert.AreEqual("¿el nodo debe poder ejecutarse en una Raspberry?", question.Name);
		}

		[Test]
		public void Members_list()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var controller = new MemberController(node.Object, new NullLogger<MemberController>());

			var members = controller.List(cryptoVoteId).ToArray();

			Assert.AreEqual(1, members.Length);

			var member = members[0];
			Assert.IsNotNull(member);
			Assert.AreEqual("Nelo", member.Name);

		}

		[Test]
		public void Member_get()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var controller = new MemberController(node.Object, new NullLogger<MemberController>());

			var member = controller.Get(cryptoVoteId, neloId);

			Assert.IsNotNull(member);
			Assert.AreEqual("Nelo", member.Name);
		}
	}
}