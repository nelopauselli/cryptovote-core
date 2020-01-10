using System;
using System.Text.Json;
using Domain.Converters;
using Domain.Crypto;
using Domain.Elections;
using Domain.Utils;
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

			var json = JsonSerializer.Serialize(community1, JsonDefaultSettings.Options);
			Console.WriteLine(json);

			var community2 = JsonSerializer.Deserialize<Community>(json, JsonDefaultSettings.Options);
			var publicKeyBase58 = Base58.Encode(community2.PublicKey);

			var verifier = new SignatureVerify(service);
			Assert.IsTrue(verifier.Verify(community2));
			Assert.IsTrue(verifier.Verify(community2.GetData(), Base58.Decode(publicKeyBase58), community2.Signature));
			Assert.IsTrue(verifier.Verify(community2.GetData(), Base58.Decode(publicKeyBase58), Base58.Decode(signatureBase58)));
		}

		[Test]
		public void Question()
		{
			var question1 = new Question
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
			question1.Signature = signer.Sign(question1.GetData(), keys);
			var signatureBase58 = Base58.Encode(question1.Signature);
			question1.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, question1.PublicKey, 0, keys.PublicKey.Length);

			var json = JsonSerializer.Serialize(question1, JsonDefaultSettings.Options);
			Console.WriteLine(json);

			var question2 = JsonSerializer.Deserialize<Question>(json, JsonDefaultSettings.Options);
			var publicKeyBase58 = Base58.Encode(question2.PublicKey);

			var verifier = new SignatureVerify(service);
			Assert.IsTrue(verifier.Verify(question1));

			Console.WriteLine(BitConverter.ToString(question1.GetData()));
			Console.WriteLine(BitConverter.ToString(question2.GetData()));
			Assert.IsTrue(verifier.Verify(question2.GetData(), Base58.Decode(publicKeyBase58), Base58.Decode(signatureBase58)));
			Assert.IsTrue(verifier.Verify(question2.GetData(), Base58.Decode(publicKeyBase58), question2.Signature));
			Assert.IsTrue(verifier.Verify(question2));
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

			var json = JsonSerializer.Serialize(member1, JsonDefaultSettings.Options);
			Console.WriteLine(json);

			var member2 = JsonSerializer.Deserialize<Member>(json, JsonDefaultSettings.Options);
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
				QuestionId = new Guid("bd746b3b276e454a8B1e041cf53a8747"),
				ChoiceId = Guid.NewGuid(),
				Time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};

			var keys = service.GeneratePair();

			var signer = new Signer(service);
			vote1.Signature = signer.Sign(vote1.GetData(), keys);
			var signatureBase58 = Base58.Encode(vote1.Signature);
			vote1.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, vote1.PublicKey, 0, keys.PublicKey.Length);

			var json = JsonSerializer.Serialize(vote1, JsonDefaultSettings.Options);
			Console.WriteLine(json);

			var vote = JsonSerializer.Deserialize<Vote>(json, JsonDefaultSettings.Options);
			var publicKeyBase58 = Base58.Encode(vote.PublicKey);

			var verifier = new SignatureVerify(service);
			Assert.IsTrue(verifier.Verify(vote));
			Assert.IsTrue(verifier.Verify(vote.GetData(), Base58.Decode(publicKeyBase58), vote.Signature));
			Assert.IsTrue(verifier.Verify(vote.GetData(), Base58.Decode(publicKeyBase58), Base58.Decode(signatureBase58)));
		}
	}
}