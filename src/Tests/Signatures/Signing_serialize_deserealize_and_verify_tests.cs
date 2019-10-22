using System;
using Domain.Crypto;
using Domain.Elections;
using Domain.Utils;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Signatures
{
	public class Signing_serialize_deserealize_and_verify_tests
	{
		private readonly ICryptoService service = new CryptoSecp256k1();

		[Test]
		public void Community()
		{
			var community1 = new Community
			{
				Id = new Guid("4814e8f885f74230b04d2daa4e2d88a4"),
				Name = "Crypto Vote",
				CreateAt = DateTimeOffset.FromUnixTimeMilliseconds(1551715934000)
			};

			var keys = service.GeneratePair();

			var signer = new Signer(service);
			community1.Signature = signer.Sign(community1.GetData(), keys);
			var signatureBase58 = Base58.Encode(community1.Signature);
			community1.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, community1.PublicKey, 0, keys.PublicKey.Length);

			var json = JsonConvert.SerializeObject(community1);
			Console.WriteLine(json);

			var community2 = JsonConvert.DeserializeObject<Community>(json);
			var publicKeyBase58 = Base58.Encode(community2.PublicKey);

			var verifier = new SignatureVerify(service);
			Assert.IsTrue(verifier.Verify(community2));
			Assert.IsTrue(verifier.Verify(community2.GetData(), Base58.Decode(publicKeyBase58), community2.Signature));
			Assert.IsTrue(verifier.Verify(community2.GetData(), Base58.Decode(publicKeyBase58), Base58.Decode(signatureBase58)));
		}

		[Test]
		public void Issue()
		{
			var issue1 = new Issue
			{
				Id = new Guid("bd746b3b276e454a8B1e041cf53a8747"),
				CommunityId = new Guid("4814e8f885f74230b04d2daa4e2d88a4"),
				Name = "El nodo debe correr en un container de Docker",
				EndTime = 1559347200000,
				Choices = new[]
				{
					new Choice {Id = Guid.NewGuid(), Text = "Si", Color = 6765239},
					new Choice {Id = Guid.NewGuid(), Text = "No", Color = 15277667}
				}
			};

			var keys = service.GeneratePair();

			var signer = new Signer(service);
			issue1.Signature = signer.Sign(issue1.GetData(), keys);
			var signatureBase58 = Base58.Encode(issue1.Signature);
			issue1.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, issue1.PublicKey, 0, keys.PublicKey.Length);

			var json = JsonConvert.SerializeObject(issue1);
			Console.WriteLine(json);

			var issue2 = JsonConvert.DeserializeObject<Issue>(json);
			var publicKeyBase58 = Base58.Encode(issue2.PublicKey);

			var verifier = new SignatureVerify(service);
			Assert.IsTrue(verifier.Verify(issue1));

			Console.WriteLine(BitConverter.ToString(issue1.GetData()));
			Console.WriteLine(BitConverter.ToString(issue2.GetData()));
			Assert.IsTrue(verifier.Verify(issue2.GetData(), Base58.Decode(publicKeyBase58), Base58.Decode(signatureBase58)));
			Assert.IsTrue(verifier.Verify(issue2.GetData(), Base58.Decode(publicKeyBase58), issue2.Signature));
			Assert.IsTrue(verifier.Verify(issue2));
		}

		[Test]
		public void Member()
		{
			var member1 = new Member
			{
				Id = new Guid("bedcbb2a476547d5b395524ffb6d157b"),
				CommunityId = new Guid("4814e8f885f74230b04d2daa4e2d88a4"),
				Name = "Nelo Pauselli"
			};

			var keys = service.GeneratePair();

			var signer = new Signer(service);
			member1.Signature = signer.Sign(member1.GetData(), keys);
			var signatureBase58 = Base58.Encode(member1.Signature);
			member1.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, member1.PublicKey, 0, keys.PublicKey.Length);

			var json = JsonConvert.SerializeObject(member1);
			Console.WriteLine(json);

			var member2 = JsonConvert.DeserializeObject<Member>(json);
			var publicKeyBase58 = Base58.Encode(member2.PublicKey);

			var verifier = new SignatureVerify(service);
			Assert.IsTrue(verifier.Verify(member2));
			Assert.IsTrue(verifier.Verify(member2.GetData(), Base58.Decode(publicKeyBase58), member2.Signature));
			Assert.IsTrue(verifier.Verify(member2.GetData(), Base58.Decode(publicKeyBase58), Base58.Decode(signatureBase58)));
		}

		[Test]
		public void Vote()
		{
			var vote1 = new Vote
			{
				IssueId = new Guid("bd746b3b276e454a8B1e041cf53a8747"),
				ChoiceId = Guid.NewGuid(),
				Time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};

			var keys = service.GeneratePair();

			var signer = new Signer(service);
			vote1.Signature = signer.Sign(vote1.GetData(), keys);
			var signatureBase58 = Base58.Encode(vote1.Signature);
			vote1.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, vote1.PublicKey, 0, keys.PublicKey.Length);

			var json = JsonConvert.SerializeObject(vote1);
			Console.WriteLine(json);

			var vote = JsonConvert.DeserializeObject<Vote>(json);
			var publicKeyBase58 = Base58.Encode(vote.PublicKey);

			var verifier = new SignatureVerify(service);
			Assert.IsTrue(verifier.Verify(vote));
			Assert.IsTrue(verifier.Verify(vote.GetData(), Base58.Decode(publicKeyBase58), vote.Signature));
			Assert.IsTrue(verifier.Verify(vote.GetData(), Base58.Decode(publicKeyBase58), Base58.Decode(signatureBase58)));
		}
	}
}