using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Channels;
using Domain.Protocol;
using Domain.Elections;
using Moq;
using NUnit.Framework;
using Tests.Mocks;

namespace Tests
{
	public class Tcp_tests : TcpTestBase
	{
		private const int DefaultSendTaskTimeout = 2000;
		private const int DefaultStartTaskTimeout = 2000;

		private string WorkingFolder => TestContext.CurrentContext.TestDirectory;

		[Test]
		public void Start_and_stop_server()
		{
			var node = new Mock<INode>();
			var channel = new TcpChannel("server", host1, port1, node.Object, new ConsoleLogger());

			try
			{
				Task.Run(() => channel.Listen());

				WaitFor(() => channel.State == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				channel.Stop();
				WaitFor(() => channel.State == ChannelState.Stop);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
			finally
			{
				if (channel.State == ChannelState.Listening)
					channel.Stop();
			}
		}

		[Test]
		public void Send_vote()
		{
			var node = new Mock<INode>();
			var channel = new TcpChannel("server", host1, port1, node.Object, new ConsoleLogger());

			try
			{
				Task.Run(() => channel.Listen());
				WaitFor(() => channel.State == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				var client = new TcpPeer(new FakeChannel("client", host2, port2), new TcpClient(host1, port1), new ConsoleLogger());

				var vote = new Vote {ChoiceId = Guid.NewGuid()};
				var command = new SendVoteCommand(vote);

				client.Send(command);
				Thread.Sleep(DefaultSendTaskTimeout);

				node.Verify(n => n.Add(It.IsAny<Vote>()), Times.Once);
			}
			finally
			{
				channel.Stop();
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Send_member()
		{
			var node = new Mock<INode>();
			var channel = new TcpChannel("server", host1, port1, node.Object, new ConsoleLogger());

			try
			{
				Task.Run(() => channel.Listen());
				WaitFor(() => channel.State == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				var client = new TcpPeer(new FakeChannel("client", host2, port2), new TcpClient(host1, port1), new ConsoleLogger());

				var member = new Member {Name = "Juan"};
				var command = new SendMemberCommand(member);

				client.Send(command);
				Thread.Sleep(DefaultSendTaskTimeout);

				node.Verify(n => n.Add(It.IsAny<Member>()), Times.Once);
			}
			finally
			{
				channel.Stop();
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Send_question()
		{
			var node = new Mock<INode>();
			Question questionInNode = null;
			node.Setup(n => n.Add(It.IsAny<Question>())).Callback<Question>(v => questionInNode = v);
			var channel = new TcpChannel("server", host1, port1, node.Object, new ConsoleLogger());

			try
			{
				Task.Run(() => channel.Listen());
				WaitFor(() => channel.State == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				var client = new TcpPeer(new FakeChannel("client", host2, port2), new TcpClient(host1, port1), new ConsoleLogger());

				var question = new Question
				{
					CommunityId = Guid.NewGuid(), Choices = new[]
					{
						new Choice {Text = "Opción Roja"},
						new Choice {Text = "Opción Azul"}
					}
				};
				var command = new SendQuestionCommand(question);

				client.Send(command);
				Thread.Sleep(DefaultSendTaskTimeout);

				node.Verify(n => n.Add(It.IsAny<Question>()), Times.Once);

				Assert.IsNotNull(questionInNode);
				Assert.AreEqual(question.CommunityId, questionInNode.CommunityId);
				Assert.AreEqual(2, questionInNode.Choices.Length);
				Assert.AreEqual(question.Choices[0].Text, question.Choices[0].Text);
				Assert.AreEqual(question.Choices[1].Text, question.Choices[1].Text);

			}
			finally
			{
				channel.Stop();
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Send_community()
		{
			var node = new Mock<INode>();
			Community communityInNode = null;
			node.Setup(n => n.Add(It.IsAny<Community>())).Callback<Community>(v => communityInNode = v);

			var channel = new TcpChannel("server", host1, port1, node.Object, new ConsoleLogger());

			try
			{
				Task.Run(() => channel.Listen());
				WaitFor(() => channel.State == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				var client = new TcpPeer(new FakeChannel("client", host2, port2), new TcpClient(host1, port1), new ConsoleLogger());

				var community = new Community
				{
					Id = Guid.NewGuid(),
					Name = "My Company"
				};

				var command = new SendCommunityCommand(community);
				client.Send(command);
				Thread.Sleep(DefaultSendTaskTimeout);

				node.Verify(n => n.Add(It.IsAny<Community>()), Times.Once);
				Assert.IsNotNull(communityInNode);
				Assert.AreEqual(community.Name, communityInNode.Name);
			}
			finally
			{
				channel.Stop();
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Send_block_2048()
		{
			var node = new Mock<INode>();
			var channel = new TcpChannel("server", host1, port1, node.Object, new ConsoleLogger());

			try
			{
				Task.Run(() => channel.Listen());
				WaitFor(() => channel.State == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				var client = new TcpPeer(new FakeChannel("client", host2, port2), new TcpClient(host1, port1), new ConsoleLogger());

				var previousHash = new byte[] {0, 0, 0, 0};
				var block = new Block(previousHash, 0);

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-2048.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(2054, content.Length);
				block.Documents.Add(new Document(content));

				var command = new SendBlockCommand(block);

				client.Send(command);
				Thread.Sleep(DefaultSendTaskTimeout);

				Thread.Sleep(1000);
				node.Verify(n => n.Add(It.IsAny<Block>(), It.IsAny<TcpPeer>()), Times.Once);
			}
			finally
			{
				channel.Stop();
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);

			}
		}

		[Test]
		public void Send_block_512()
		{
			var node = new Mock<INode>();
			var channel = new TcpChannel("server", host1, port1, node.Object, new ConsoleLogger());


			try
			{
				Task.Run(() => channel.Listen());
				WaitFor(() => channel.State == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, channel.State);
				
				var client = new TcpPeer(new FakeChannel("client", host2, port2), new TcpClient(host1, port1), new ConsoleLogger());

				var previousHash = new byte[] {0, 0, 0, 0};
				var block = new Block(previousHash, 0);

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-512.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(512, content.Length);
				block.Documents.Add(new Document(content));

				var command = new SendBlockCommand(block);
				client.Send(command);
				Thread.Sleep(DefaultSendTaskTimeout);

				Thread.Sleep(1000);
				node.Verify(n => n.Add(It.IsAny<Block>(), It.IsAny<TcpPeer>()), Times.Once);
			}
			finally
			{
				channel.Stop();
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Send_document_small()
		{
			var node = new Mock<INode>();
			Document documentInNode = null;
			node.Setup(n => n.Add(It.IsAny<Document>())).Callback<Document>(v => documentInNode = v);
			var channel = new TcpChannel("server", host1, port1, node.Object, new ConsoleLogger());

			try
			{
				Task.Run(() => channel.Listen());
				WaitFor(() => channel.State == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				var client = new TcpPeer(new FakeChannel("client", host2, port2), new TcpClient(host1, port1), new ConsoleLogger());

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-512.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(512, content.Length);
				var document = new Document(content);

				var command = new SendDocumentCommand(document);
				client.Send(command);
				Thread.Sleep(DefaultSendTaskTimeout);

				node.Verify(n => n.Add(It.IsAny<Document>()), Times.Once);
				Assert.IsNotNull(documentInNode);
				Assert.AreEqual(document.Text, documentInNode.Text);
			}
			finally
			{
				channel.Stop();
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}

		[Test]
		public void Send_document_large()
		{
			var node = new Mock<INode>();
			Document documentInNode = null;
			node.Setup(n => n.Add(It.IsAny<Document>())).Callback<Document>(v => documentInNode = v);
			var channel = new TcpChannel("server", host1, port1, node.Object, new ConsoleLogger());

			try
			{
				Task.Run(() => channel.Listen());
				WaitFor(() => channel.State == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				var client = new TcpPeer(new FakeChannel("client", host2, port2), new TcpClient(host1, port1), new ConsoleLogger());

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-2048.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(2054, content.Length);
				var document = new Document(content);

				var command = new SendDocumentCommand(document);
				client.Send(command);
				Thread.Sleep(DefaultSendTaskTimeout);

				node.Verify(n => n.Add(It.IsAny<Document>()), Times.Once);
				Assert.IsNotNull(documentInNode);
				Assert.AreEqual(document.Text, documentInNode.Text);
			}
			finally
			{
				channel.Stop();
				WaitFor(() => channel.State != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, channel.State);
			}
		}
	}
}