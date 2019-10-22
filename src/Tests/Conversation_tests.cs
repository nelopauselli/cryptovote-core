using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Domain;
using Domain.Channels;
using Domain.Crypto;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;
using NUnit.Framework;
using Tests.Mocks;

namespace Tests
{
	public class Commands_tests
	{
		[Test]
		public void Communities_list()
		{
			var factory = new CommunitiesQueryMessage();
			var message = factory.GetBytes();
			Assert.AreEqual(Encoding.UTF8.GetBytes("?:00011|Communities"), message);
		}
	}

	public class Conversation_tests
	{
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

			var member = new Member { Id = neloId, CommunityId = cryptoVote.Id, Name = "Nelo" };
			signer.Sign(member, nelo);

			blockchain.MineNextBlock(new BlockItem[] {cryptoVote, copperadora});
			var block1 = blockchain.MineNextBlock(new BlockItem[] {question, member });
			block1Hash = block1.Hash;
		}

		[Test]
		public void Get_last_block()
		{
			var conversation = new Conversation(blockchain, new IEventListener[] { new ConsoleListener() }, new ConsoleListener(), new MockChannel());
			var factory = new LastBlockQueryMessage();

			using (var stream = new MemoryStream())
			{
				var message = factory.GetBytes();
				stream.Write(message);
				stream.Flush();

				var position = stream.Position;
				stream.Seek(0, SeekOrigin.Begin);

				conversation.Talk(new ProtocolMessageChannel(stream));

				stream.Seek(position, SeekOrigin.Begin);
				var reader = new ProtocolMessageChannel(stream);
				Assert.AreEqual(SendBlockMessage.CommandId, reader.GetCommandId());
				
				var block = factory.Parse(reader);
				Assert.IsNotNull(block);
				Assert.AreEqual(1, block.Questions.Count);
				Assert.AreEqual(1, block.Members.Count);
			}
		}

		[Test]
		public void Get_block()
		{
			var conversation = new Conversation(blockchain, new IEventListener[] { new ConsoleListener() }, new ConsoleListener(), new MockChannel());
			var factory = new BlockQueryMessage(block1Hash);

			using (var stream = new MemoryStream())
			{
				var message = factory.GetBytes();
				stream.Write(message);
				stream.Flush();

				var position = stream.Position;
				stream.Seek(0, SeekOrigin.Begin);

				conversation.Talk(new ProtocolMessageChannel(stream));

				stream.Seek(position, SeekOrigin.Begin);
				var reader = new ProtocolMessageChannel(stream);
				Assert.AreEqual('B', reader.GetCommandId());

				var block = factory.Parse(reader);
				Assert.IsNotNull(block);
				Assert.AreEqual(1, block.Questions.Count);
				Assert.AreEqual(1, block.Members.Count);
			}
		}

		[Test]
		public void Communities_list()
		{
			var conversation = new Conversation(blockchain, new IEventListener[] {new ConsoleListener()}, new ConsoleListener(), new MockChannel());
			var factory = new CommunitiesQueryMessage();

			using (var stream = new MemoryStream())
			{
				var message = factory.GetBytes();
				stream.Write(message);
				stream.Flush();

				var position = stream.Position;
				stream.Seek(0, SeekOrigin.Begin);

				conversation.Talk(new ProtocolMessageChannel(stream));

				stream.Seek(position, SeekOrigin.Begin);
				var communities = factory.Parse(new ProtocolMessageChannel(stream));
				Assert.AreEqual(2, communities.Length);
			}
		}

		[Test]
		public void Community_get()
		{
			var conversation = new Conversation(blockchain, new IEventListener[] { new ConsoleListener() }, new ConsoleListener(), new MockChannel());
			var factory = new CommunityQueryMessage(cryptoVoteId);

			using (var stream = new MemoryStream())
			{
				var message = factory.GetBytes();
				stream.Write(message);
				stream.Flush();

				var position = stream.Position;
				stream.Seek(0, SeekOrigin.Begin);

				conversation.Talk(new ProtocolMessageChannel(stream));

				stream.Seek(position, SeekOrigin.Begin);
				var community = factory.Parse(new ProtocolMessageChannel(stream));
				Assert.IsNotNull(community);
				Assert.AreEqual("Crypto Vote", community.Name);
			}
		}

		[Test]
		public void Questions_list()
		{
			var conversation = new Conversation(blockchain, new IEventListener[] {new ConsoleListener()}, new ConsoleListener(), new MockChannel());
			var factory = new QuestionsQueryMessage(cryptoVoteId);

			using (var stream = new MemoryStream())
			{
				var message = factory.GetBytes();
				stream.Write(message);
				stream.Flush();

				var position = stream.Position;
				stream.Seek(0, SeekOrigin.Begin);

				conversation.Talk(new ProtocolMessageChannel(stream));

				stream.Seek(position, SeekOrigin.Begin);
				var questions = factory.Parse(new ProtocolMessageChannel(stream));
				Assert.AreEqual(1, questions.Length);

				var question = questions[0];
				Assert.IsNotNull(question);
				Assert.AreEqual("¿el nodo debe poder ejecutarse en una Raspberry?", question.Name);
			}
		}

		[Test]
		public void Question_get()
		{
			var conversation = new Conversation(blockchain, new IEventListener[] { new ConsoleListener() }, new ConsoleListener(), new MockChannel());
			var factory = new QuestionQueryMessage(cryptoVoteId, questionId);

			using (var stream = new MemoryStream())
			{
				var message = factory.GetBytes();
				stream.Write(message);
				stream.Flush();

				var position = stream.Position;
				stream.Seek(0, SeekOrigin.Begin);

				conversation.Talk(new ProtocolMessageChannel(stream));

				stream.Seek(position, SeekOrigin.Begin);
				var question = factory.Parse(new ProtocolMessageChannel(stream));
				Assert.IsNotNull(question);
				Assert.AreEqual("¿el nodo debe poder ejecutarse en una Raspberry?", question.Name);
			}
		}

		[Test]
		public void Members_list()
		{
			var conversation = new Conversation(blockchain, new IEventListener[] { new ConsoleListener() }, new ConsoleListener(), new MockChannel());
			var factory = new MembersQueryMessage(cryptoVoteId);

			using (var stream = new MemoryStream())
			{
				var message = factory.GetBytes();
				stream.Write(message);
				stream.Flush();

				var position = stream.Position;
				stream.Seek(0, SeekOrigin.Begin);

				conversation.Talk(new ProtocolMessageChannel(stream));

				stream.Seek(position, SeekOrigin.Begin);
				var members = factory.Parse(new ProtocolMessageChannel(stream));
				Assert.AreEqual(1, members.Length);

				var member = members[0];
				Assert.IsNotNull(member);
				Assert.AreEqual("Nelo", member.Name);
			}
		}

		[Test]
		public void Member_get()
		{
			var conversation = new Conversation(blockchain, new IEventListener[] { new ConsoleListener() }, new ConsoleListener(), new MockChannel());
			var factory = new MemberQueryMessage(cryptoVoteId, neloId);

			using (var stream = new MemoryStream())
			{
				var message = factory.GetBytes();
				stream.Write(message);
				stream.Flush();

				var position = stream.Position;
				stream.Seek(0, SeekOrigin.Begin);

				conversation.Talk(new ProtocolMessageChannel(stream));

				stream.Seek(position, SeekOrigin.Begin);
				var question = factory.Parse(new ProtocolMessageChannel(stream));
				Assert.IsNotNull(question);
				Assert.AreEqual("Nelo", question.Name);
			}
		}
	}

	public class MockChannel : IChannel
	{
		public void TalkWithClient(Stream stream)
		{
		}
	}
}