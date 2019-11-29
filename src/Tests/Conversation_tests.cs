using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Crypto;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;
using Moq;
using NUnit.Framework;
using Tests.Mocks;

namespace Tests
{
	public class Conversation_tests : TcpTestBase
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
			var channel = new TcpChannel("server", host, port, node.Object, new ConsoleLogger());

			Task.Run(() => channel.Listen());
			WaitFor(() => channel.State != ChannelState.Stop);
			Assert.AreEqual(ChannelState.Listening, channel.State);

			try
			{
				var command = new GetLastBlockCommand();

				using (var client = new TcpClient(host, port))
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);

						Thread.Sleep(1000);

						var header = CommandHeader.Parse(stream);
						Assert.IsNotNull(header);
						Assert.AreEqual(CommandIds.SendBlock, header.CommandId);

						var buffer = new byte[header.Length];
						stream.Read(buffer, 0, header.Length);

						var block = Serializer.Parse<Block>(buffer);

						Assert.IsNotNull(block);
						Assert.AreEqual(1, block.Questions.Count);
						Assert.AreEqual(1, block.Members.Count);
					}
				}
			}
			finally
			{
				Task.Run(() => channel.Stop());
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Get_block()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var channel = new TcpChannel("server", host, port, node.Object, new ConsoleLogger());

			Task.Run(() => channel.Listen());
			WaitFor(() => channel.State != ChannelState.Stop);
			Assert.AreEqual(ChannelState.Listening, channel.State);

			try
			{
				var command = new GetBlockCommand(block1Hash);

				using (var client = new TcpClient(host, port))
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);

						Thread.Sleep(1000);

						var header = CommandHeader.Parse(stream);
						Assert.IsNotNull(header);
						Assert.AreEqual(CommandIds.SendBlock, header.CommandId);

						var buffer = new byte[header.Length];
						stream.Read(buffer, 0, header.Length);

						var block = Serializer.Parse<Block>(buffer);
						Assert.IsNotNull(block);
						Assert.AreEqual(1, block.Questions.Count);
						Assert.AreEqual(1, block.Members.Count);
					}
				}
			}
			finally
			{
				Task.Run(() => channel.Stop());
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Communities_list()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var channel = new TcpChannel("server", host, port, node.Object, new ConsoleLogger());

			Task.Run(() => channel.Listen());
			WaitFor(() => channel.State != ChannelState.Stop);
			Assert.AreEqual(ChannelState.Listening, channel.State);

			try
			{
				var command = new CommunitiesQueryCommand();

				using (var client = new TcpClient(host, port))
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);

						Thread.Sleep(1000);

						var header = CommandHeader.Parse(stream);
						Assert.IsNotNull(header);
						Assert.AreEqual(CommandIds.QueryResponse, header.CommandId);

						var communities = command.Parse(stream, header.Length);

						Assert.IsNotNull(communities);
						Assert.AreEqual(2, communities.Length);

					}
				}
			}
			finally
			{
				Task.Run(() => channel.Stop());
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Community_get()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var channel = new TcpChannel("server", host, port, node.Object, new ConsoleLogger());

			Task.Run(() => channel.Listen());
			WaitFor(() => channel.State != ChannelState.Stop);
			Assert.AreEqual(ChannelState.Listening, channel.State);

			try
			{
				var command = new CommunityQueryCommand(cryptoVoteId);

				using (var client = new TcpClient(host, port))
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);

						Thread.Sleep(1000);

						var header = CommandHeader.Parse(stream);
						Assert.IsNotNull(header);
						Assert.AreEqual(CommandIds.QueryResponse, header.CommandId);

						var community = command.Parse(stream, header.Length);

						Assert.IsNotNull(community);
						Assert.AreEqual("Crypto Vote", community.Name);
					}
				}
			}
			finally
			{
				Task.Run(() => channel.Stop());
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Questions_list()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var channel = new TcpChannel("server", host, port, node.Object, new ConsoleLogger());

			Task.Run(() => channel.Listen());
			WaitFor(() => channel.State != ChannelState.Stop);
			Assert.AreEqual(ChannelState.Listening, channel.State);

			try
			{
				var command = new QuestionsQueryCommand(cryptoVoteId);

				using (var client = new TcpClient(host, port))
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);

						Thread.Sleep(1000);

						var header = CommandHeader.Parse(stream);
						Assert.IsNotNull(header);
						Assert.AreEqual(CommandIds.QueryResponse, header.CommandId);

						var buffer = new byte[header.Length];
						stream.Read(buffer, 0, header.Length);

						var questions = Serializer.Parse<Question[]>(buffer);

						Assert.AreEqual(1, questions.Length);

						var question = questions[0];
						Assert.IsNotNull(question);
						Assert.AreEqual("¿el nodo debe poder ejecutarse en una Raspberry?", question.Name);
					}
				}
			}
			finally
			{
				Task.Run(() => channel.Stop());
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Question_get()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var channel = new TcpChannel("server", host, port, node.Object, new ConsoleLogger());

			Task.Run(() => channel.Listen());
			WaitFor(() => channel.State != ChannelState.Stop);
			Assert.AreEqual(ChannelState.Listening, channel.State);

			try
			{
				var command = new QuestionQueryCommand(cryptoVoteId, questionId);

				using (var client = new TcpClient(host, port))
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);

						Thread.Sleep(1000);

						var header = CommandHeader.Parse(stream);
						Assert.IsNotNull(header);
						Assert.AreEqual(CommandIds.QueryResponse, header.CommandId);

						var buffer = new byte[header.Length];
						stream.Read(buffer, 0, header.Length);

						var question = Serializer.Parse<Question>(buffer);

						Assert.IsNotNull(question);
						Assert.AreEqual("¿el nodo debe poder ejecutarse en una Raspberry?", question.Name);
					}
				}
			}
			finally
			{
				Task.Run(() => channel.Stop());
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Members_list()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var channel = new TcpChannel("server", host, port, node.Object, new ConsoleLogger());

			Task.Run(() => channel.Listen());
			WaitFor(() => channel.State != ChannelState.Stop);
			Assert.AreEqual(ChannelState.Listening, channel.State);

			try
			{
				var command = new MembersQueryCommand(cryptoVoteId);

				using (var client = new TcpClient(host, port))
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);

						Thread.Sleep(1000);

						var header = CommandHeader.Parse(stream);
						Assert.IsNotNull(header);
						Assert.AreEqual(CommandIds.QueryResponse, header.CommandId);

						var buffer = new byte[header.Length];
						stream.Read(buffer, 0, header.Length);

						var members = Serializer.Parse<Member[]>(buffer);

						Assert.AreEqual(1, members.Length);

						var member = members[0];
						Assert.IsNotNull(member);
						Assert.AreEqual("Nelo", member.Name);
					}
				}
			}
			finally
			{
				Task.Run(() => channel.Stop());
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Member_get()
		{
			var node = new Mock<INode>();
			node.SetupGet(n => n.Blockchain).Returns(blockchain);
			var channel = new TcpChannel("server", host, port, node.Object, new ConsoleLogger());

			Task.Run(() => channel.Listen());
			WaitFor(() => channel.State != ChannelState.Stop);
			Assert.AreEqual(ChannelState.Listening, channel.State);

			try
			{
				var command = new MemberQueryCommand(cryptoVoteId, neloId);

				using (var client = new TcpClient(host, port))
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);

						Thread.Sleep(1000);

						var header = CommandHeader.Parse(stream);
						Assert.IsNotNull(header);
						Assert.AreEqual(CommandIds.QueryResponse, header.CommandId);

						var buffer = new byte[header.Length];
						stream.Read(buffer, 0, header.Length);

						var member = Serializer.Parse<Member>(buffer);

						Assert.IsNotNull(member);
						Assert.AreEqual("Nelo", member.Name);
					}
				}
			}
			finally
			{
				Task.Run(() => channel.Stop());
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}
	}
}