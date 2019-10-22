using System;
using System.Linq;
using Domain.Elections;
using Domain.Utils;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Serealization
{
	[TestFixture]
	public class Vote_json_serealization_tests
	{
		private readonly Guid issueId = Guid.NewGuid();
		private readonly string publicKeyEncoded = "aSq9DsNNvGhYxYyqA9wd2eduEAZ5AXWgJTbTFuhuNNFx88DbyZsX6bSVtwctxw71z1QdeQWqqe66RXcFUHcn2nWypsaCF1u2UFJhifLcKMHmDbvq3g1eMiLMnMQJ";
		private readonly string signatureEncoded = "iKx1CJMxPB4htfudCB5zpsKqLLz2V9mDqNXVF428w9emRb7TsR9bgW4zVYGYbh38VdDea4mz8uuQbGeXLTYnWuNtTM5qwbyLxU";
		private readonly Guid choiceId = Guid.NewGuid();
		private readonly long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

		private Vote vote;

		[SetUp]
		public void Init()
		{
			vote = new Vote
			{
				IssueId = issueId, ChoiceId = choiceId, Time = time,
				PublicKey = Base58.Decode(publicKeyEncoded),
				Signature = Base58.Decode(signatureEncoded)
			};
		}

		[Test]
		public void IssueId()
		{
			var serialized = JsonConvert.SerializeObject(vote);
			var vote2 = JsonConvert.DeserializeObject<Vote>(serialized);
			
			Assert.IsNotNull(vote2);
			Assert.AreEqual(issueId, vote2.IssueId);
		}

		[Test]
		public void Time()
		{
			var serialized = JsonConvert.SerializeObject(vote);
			var vote2 = JsonConvert.DeserializeObject<Vote>(serialized);

			Assert.IsNotNull(vote2);
			Assert.AreEqual(time, vote2.Time);
		}

		[Test]
		public void Choice()
		{
			var serialized = JsonConvert.SerializeObject(vote);
			var vote2 = JsonConvert.DeserializeObject<Vote>(serialized);

			Assert.IsNotNull(vote2);
			Assert.AreEqual(choiceId, vote2.ChoiceId);
		}

		[Test]
		public void PublicKey()
		{
			var serialized = JsonConvert.SerializeObject(vote);
			var vote2 = JsonConvert.DeserializeObject<Vote>(serialized);

			Assert.IsNotNull(vote2);

			var publicKey = Base58.Decode(publicKeyEncoded);
			Assert.IsTrue(publicKey.SequenceEqual(vote2.PublicKey));
		}

		[Test]
		public void Signature()
		{
			var serialized = JsonConvert.SerializeObject(vote);
			var vote2 = JsonConvert.DeserializeObject<Vote>(serialized);

			Assert.IsNotNull(vote2);

			var signature = Base58.Decode(signatureEncoded);
			Assert.IsTrue(signature.SequenceEqual(vote2.Signature));
		}
	}
}