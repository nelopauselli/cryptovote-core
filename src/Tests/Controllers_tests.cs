using System;
using System.IO;
using System.Net;
using CryptoVote.Controllers;
using Domain;
using Domain.Elections;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests
{
	public class Controllers_tests : TestWithServices
	{
		private string WorkingFolder => TestContext.CurrentContext.TestDirectory;

		[Test]
		public void Send_vote()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var node = new Mock<INode>();
				var controller = new VoteController(node.Object, loggerFactory.CreateLogger<VoteController>());

				var vote = new Vote {ChoiceId = Guid.NewGuid()};
				var response = controller.Post(vote);

				Assert.AreEqual((int) HttpStatusCode.Accepted, response.StatusCode);

				node.Verify(n => n.Add(It.IsAny<Vote>()), Times.Once);
			}
		}

		[Test]
		public void Send_member()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var node = new Mock<INode>();
				var controller = new MemberController(node.Object, loggerFactory.CreateLogger<MemberController>());

				var member = new Member {CommunityId = Guid.NewGuid(), Id = Guid.NewGuid(), Name = "Juan"};
				var response = controller.Post(member);

				Assert.AreEqual((int) HttpStatusCode.Accepted, response.StatusCode);

				node.Verify(n => n.Add(It.IsAny<Member>()), Times.Once);
			}
		}

		[Test]
		public void Send_question()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var node = new Mock<INode>();
				Question questionInNode = null;
				node.Setup(n => n.Add(It.IsAny<Question>())).Callback<Question>(v => questionInNode = v);
				var controller = new QuestionController(node.Object, loggerFactory.CreateLogger<QuestionController>());
				
				var question = new Question
				{
					CommunityId = Guid.NewGuid(), Choices = new[]
					{
						new Choice {Text = "Opción Roja"},
						new Choice {Text = "Opción Azul"}
					}
				};
				var response = controller.Post(question);

				Assert.AreEqual((int) HttpStatusCode.Accepted, response.StatusCode);
				node.Verify(n => n.Add(It.IsAny<Question>()), Times.Once);

				Assert.IsNotNull(questionInNode);
				Assert.AreEqual(question.CommunityId, questionInNode.CommunityId);
				Assert.AreEqual(2, questionInNode.Choices.Length);
				Assert.AreEqual(question.Choices[0].Text, question.Choices[0].Text);
				Assert.AreEqual(question.Choices[1].Text, question.Choices[1].Text);
			}
		}

		[Test]
		public void Send_community()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var node = new Mock<INode>();
				Community communityInNode = null;
				node.Setup(n => n.Add(It.IsAny<Community>())).Callback<Community>(v => communityInNode = v);
				var controller = new CommunityController(node.Object, loggerFactory.CreateLogger<CommunityController>());

				var community = new Community
				{
					Id = Guid.NewGuid(),
					Name = "My Company"
				};

				var response = controller.Post(community);

				Assert.AreEqual((int) HttpStatusCode.Accepted, response.StatusCode);

				node.Verify(n => n.Add(It.IsAny<Community>()), Times.Once);
				Assert.IsNotNull(communityInNode);
				Assert.AreEqual(community.Name, communityInNode.Name);
			}
		}

		[Test]
		public void Send_peer()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var node = new Mock<INode>();
				Peer peerInNode = null;
				node.Setup(n => n.Register(It.IsAny<Peer>())).Callback<Peer>(v => peerInNode = v);
				var controller = new PeerController(node.Object, loggerFactory.CreateLogger<PeerController>());

				var peer = new Peer
				{
					Id = Guid.NewGuid(),
					Name = "My Node",
					PublicUrl = "http://mynode:1234"
				};

				var response = controller.Post(peer);

				Assert.AreEqual((int) HttpStatusCode.Accepted, response.StatusCode);

				node.Verify(n => n.Register(It.IsAny<Peer>()), Times.Once);
				Assert.IsNotNull(peerInNode);
				Assert.AreEqual(peer.Name, peerInNode.Name);
				Assert.AreEqual(peer.PublicUrl, peerInNode.PublicUrl);
				Assert.AreEqual(peer.Id, peerInNode.Id);
			}
		}

		[Test]
		public void Send_block_2048()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var node = new Mock<INode>();
				var controller = new ChainController(node.Object, loggerFactory.CreateLogger<ChainController>());

				var previousHash = new byte[] {0, 0, 0, 0};
				var block = new Block(previousHash, 0);

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-2048.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(2054, content.Length);

				block.Documents.Add(new Document(content));

				var response = controller.Post(block);

				Assert.AreEqual((int) HttpStatusCode.Accepted, response.StatusCode);
				node.Verify(n => n.Add(It.IsAny<Block>()), Times.Once);
			}
		}

		[Test]
		public void Send_block_512()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var node = new Mock<INode>();
				var controller = new ChainController(node.Object, loggerFactory.CreateLogger<ChainController>());

				var previousHash = new byte[] {0, 0, 0, 0};
				var block = new Block(previousHash, 0);

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-512.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(512, content.Length);
				block.Documents.Add(new Document(content));

				var response = controller.Post(block);

				Assert.AreEqual((int) HttpStatusCode.Accepted, response.StatusCode);
				node.Verify(n => n.Add(It.IsAny<Block>()), Times.Once);
			}
		}

		[Test]
		public void Send_document_small()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var node = new Mock<INode>();
				Document documentInNode = null;
				node.Setup(n => n.Add(It.IsAny<Document>())).Callback<Document>(v => documentInNode = v);
				var controller = new DocumentController(node.Object, loggerFactory.CreateLogger<DocumentController>());


				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-512.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(512, content.Length);
				var document = new Document(content);

				var response = controller.Post(document);
				Assert.AreEqual((int) HttpStatusCode.Accepted, response.StatusCode);

				node.Verify(n => n.Add(It.IsAny<Document>()), Times.Once);
				Assert.IsNotNull(documentInNode);
				Assert.AreEqual(document.Text, documentInNode.Text);
			}
		}

		[Test]
		public void Send_document_large()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var node = new Mock<INode>();
				Document documentInNode = null;
				node.Setup(n => n.Add(It.IsAny<Document>())).Callback<Document>(v => documentInNode = v);
				var controller = new DocumentController(node.Object, loggerFactory.CreateLogger<DocumentController>());

				var path = Path.Combine(WorkingFolder, "files", "Lorem-Ipsum-2048.txt");
				Assert.IsTrue(File.Exists(path));
				var content = File.ReadAllText(path);
				Assert.AreEqual(2054, content.Length);
				var document = new Document(content);

				var response = controller.Post(document);
				Assert.AreEqual((int) HttpStatusCode.Accepted, response.StatusCode);

				node.Verify(n => n.Add(It.IsAny<Document>()), Times.Once);
				Assert.IsNotNull(documentInNode);
				Assert.AreEqual(document.Text, documentInNode.Text);
			}
		}
	}
}