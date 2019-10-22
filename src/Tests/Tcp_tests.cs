using System;
using System.IO;
using System.Net;
using System.Threading;
using Domain;
using Domain.Channels;
using Domain.Protocol;
using Domain.Elections;
using Moq;
using NUnit.Framework;

namespace Tests
{
	public class Tcp_tests:TcpTestBase
	{
		private const int DefaultSendTaskTimeout = 2000;
		private const int DefaultStartTaskTimeout = 2000;
		
		private string WorkingFolder => TestContext.CurrentContext.TestDirectory;

		private readonly IPAddress address = new IPAddress(new byte[] {127, 0, 0, 1});

		[Test]
		public void Start_and_stop_server()
		{
			var node = new Mock<INode>();
			var channel = new TcpChannel(node.Object, port1, new ConsoleListener());

			try
			{
				channel.Start(DefaultStartTaskTimeout);
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
			var channel = new TcpChannel(node.Object, port1, new ConsoleListener());

			try
			{
				channel.Start(DefaultStartTaskTimeout);
				WaitFor(() => channel.State == ChannelState.Listening);

				var client = new TcpPeer(address.ToString(), port1, new MockChannel());

				var vote = new Vote {ChoiceId = Guid.NewGuid()};
				var command = new SendVoteMessage(vote);

				var sendTask = client.SendAsync(command.GetBytes(), CancellationToken.None);
				sendTask.Wait(DefaultSendTaskTimeout);

				node.Verify(n=>n.Add(It.IsAny<Vote>()), Times.Once);
			}
			finally
			{
				if (channel.State== ChannelState.Listening)
					channel.Stop();
			}
		}

		[Test]
		public void Send_member()
		{
			var node = new Mock<INode>();
			var channel = new TcpChannel(node.Object, port1, new ConsoleListener());

			try
			{
				channel.Start(DefaultStartTaskTimeout);
				WaitFor(() => channel.State == ChannelState.Listening);

				var client = new TcpPeer(address.ToString(), port1, new MockChannel());

				var member = new Member {Name = "Juan"};
				var command = new SendMemberMessage(member);

				var sendTask = client.SendAsync(command.GetBytes(), CancellationToken.None);
				sendTask.Wait(DefaultSendTaskTimeout);

				node.Verify(n => n.Add(It.IsAny<Member>()), Times.Once);
			}
			finally
			{
				if (channel.State == ChannelState.Listening)
					channel.Stop();
			}
		}

		[Test]
		public void Send_question()
		{
			var node = new Mock<INode>();
			Question questionInNode = null;
			node.Setup(n => n.Add(It.IsAny<Question>())).Callback<Question>(v => questionInNode = v);
			var channel = new TcpChannel(node.Object, port1, new ConsoleListener());

			try
			{
				channel.Start(DefaultStartTaskTimeout);
				WaitFor(() => channel.State == ChannelState.Listening);

				var client = new TcpPeer(address.ToString(), port1, new MockChannel());

				var question = new Question
				{
					CommunityId = Guid.NewGuid(), Choices = new[]
					{
						new Choice {Text = "Opción Roja"},
						new Choice {Text = "Opción Azul"}
					}
				};
				var command = new SendQuestionMessage(question);

				var sendTask = client.SendAsync(command.GetBytes(), CancellationToken.None);
				sendTask.Wait(DefaultSendTaskTimeout);

				node.Verify(n => n.Add(It.IsAny<Question>()), Times.Once);

				Assert.IsNotNull(questionInNode);
				Assert.AreEqual(question.CommunityId, questionInNode.CommunityId);
				Assert.AreEqual(2, questionInNode.Choices.Length);
				Assert.AreEqual(question.Choices[0].Text, question.Choices[0].Text);
				Assert.AreEqual(question.Choices[1].Text, question.Choices[1].Text);

			}
			finally
			{
				if (channel.State == ChannelState.Listening)
					channel.Stop();
			}
		}

		[Test]
		public void Send_community()
		{
			var node = new Mock<INode>();
			Community communityInNode = null;
			node.Setup(n => n.Add(It.IsAny<Community>())).Callback<Community>(v => communityInNode = v);
			var channel = new TcpChannel(node.Object, port1, new ConsoleListener());

			try
			{
				channel.Start(DefaultStartTaskTimeout);
				WaitFor(() => channel.State == ChannelState.Listening);

				var client = new TcpPeer(address.ToString(), port1, new MockChannel());

				var community = new Community
				{
					Id = Guid.NewGuid(),
					Name = "My Company"
				};
				
				var command = new SendCommunityMessage(community);
				var sendTask = client.SendAsync(command.GetBytes(), CancellationToken.None);
				sendTask.Wait(DefaultSendTaskTimeout);

				node.Verify(n=>n.Add(It.IsAny<Community>()), Times.Once);
				Assert.IsNotNull(communityInNode);
				Assert.AreEqual(community.Name, communityInNode.Name);
			}
			finally
			{
				if (channel.State == ChannelState.Listening)
					channel.Stop();
			}
		}

		[Test]
		public void Send_peer()
		{
			var node = new Mock<INode>();

			var server = new TcpChannel(node.Object, port1, new ConsoleListener());
			server.AddListener(new ConsoleListener());

			try
			{
				server.Start(DefaultStartTaskTimeout);

				var client = new TcpPeer(address.ToString(), port1, new MockChannel());

				var peer = new PeerInfo
				{
					Host = "1.2.3.4",
					Port = 3333
				};
				var command = new SendPeerInfoMessage(peer);

				var sendTask = client.SendAsync(command.GetBytes(), CancellationToken.None);
				sendTask.Wait(DefaultSendTaskTimeout);

				Thread.Sleep(100);
				node.Verify(n => n.Register(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

				server.Stop();
			}
			finally
			{
				if (server.State == ChannelState.Listening)
					server.Stop();
			}
		}

		[Test]
		public void Send_block_2048()
		{
			var node = new Mock<INode>();
			var channel = new TcpChannel(node.Object, port1, new ConsoleListener());

			try
			{
				channel.Start(DefaultStartTaskTimeout);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				var client = new TcpPeer(address.ToString(), port1, new MockChannel());

				var previousHash = new byte[] {0, 0, 0, 0};
				var block = new Block(previousHash, 0);

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-2048.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(2054, content.Length);
				block.Documents.Add(new Document(content));

				var command = new SendBlockMessage(block);

				var sendTask = client.SendAsync(command.GetBytes(), CancellationToken.None);
				sendTask.Wait(4000);

				Thread.Sleep(1000);
				node.Verify(n => n.Add(It.IsAny<Block>()), Times.Once);

				channel.Stop();
			}
			finally
			{
				if (channel.State == ChannelState.Listening)
					channel.Stop();
			}
		}

		[Test]
		public void Send_block_512()
		{
			var node = new Mock<INode>();
			var channel = new TcpChannel(node.Object, port1, new ConsoleListener());

			channel.AddListener(new ConsoleListener());

			try
			{
				channel.Start(DefaultStartTaskTimeout);
				Assert.AreEqual(ChannelState.Listening, channel.State);

				var client = new TcpPeer(address.ToString(), port1, new MockChannel());

				var previousHash = new byte[] {0, 0, 0, 0};
				var block = new Block(previousHash, 0);

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-512.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(512, content.Length);
				block.Documents.Add(new Document(content));

				var command = new SendBlockMessage(block);
				var sendTask = client.SendAsync(command.GetBytes(), CancellationToken.None);
				sendTask.Wait(4000);

				Thread.Sleep(1000);
				node.Verify(n => n.Add(It.IsAny<Block>()), Times.Once);
			}
			finally
			{
				if (channel.State == ChannelState.Listening)
					channel.Stop();
			}
		}

		[Test]
		public void Send_document_small()
		{
			var node = new Mock<INode>();
			Document documentInNode = null;
			node.Setup(n => n.Add(It.IsAny<Document>())).Callback<Document>(v => documentInNode = v);
			var channel = new TcpChannel(node.Object, port1, new ConsoleListener());

			try
			{
				channel.Start(DefaultStartTaskTimeout);
				WaitFor(() => channel.State == ChannelState.Listening);

				var client = new TcpPeer(address.ToString(), port1, new MockChannel());

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-512.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(512, content.Length);
				var document = new Document(content);

				var command = new SendDocumentMessage(document);
				var sendTask = client.SendAsync(command.GetBytes(), CancellationToken.None);
				sendTask.Wait(4000);

				node.Verify(n=>n.Add(It.IsAny<Document>()), Times.Once);
				Assert.IsNotNull(documentInNode);
				Assert.AreEqual(document.Text, documentInNode.Text);
			}
			finally
			{
				if (channel.State == ChannelState.Listening)
					channel.Stop();
			}
		}

		[Test]
		public void Send_document_large()
		{
			var node = new Mock<INode>();
			Document documentInNode = null;
			node.Setup(n => n.Add(It.IsAny<Document>())).Callback<Document>(v => documentInNode = v);
			var channel = new TcpChannel(node.Object, port1, new ConsoleListener());

			try
			{
				channel.Start(DefaultStartTaskTimeout);
				WaitFor(() => channel.State == ChannelState.Listening);

				var client = new TcpPeer(address.ToString(), port1, new MockChannel());

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-2048.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(2054, content.Length);
				var document = new Document(content);

				var command = new SendDocumentMessage(document);
				var sendTask = client.SendAsync(command.GetBytes(), CancellationToken.None);
				sendTask.Wait(4000);

				node.Verify(n=>n.Add(It.IsAny<Document>()), Times.Once);
				Assert.IsNotNull(documentInNode);
				Assert.AreEqual(document.Text, documentInNode.Text);
			}
			finally
			{
				if (channel.State == ChannelState.Listening)
					channel.Stop();
			}
		}
	}
}