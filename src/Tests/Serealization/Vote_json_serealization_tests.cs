using System;
using System.Linq;
using System.Text.Json;
using Domain.Converters;
using Domain.Elections;
using Domain.Utils;
using NUnit.Framework;

namespace Tests.Serealization
{
	[TestFixture]
	public class Vote_json_serealization_tests
	{
		private readonly Guid questionId = Guid.NewGuid();
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
				QuestionId = questionId, ChoiceId = choiceId, Time = time,
				PublicKey = Base58.Decode(publicKeyEncoded),
				Signature = Base58.Decode(signatureEncoded)
			};
		}

		[Test]
		public void QuestionId()
		{
			var serialized = JsonSerializer.Serialize(vote, JsonDefaultSettings.Options);
			var vote2 = JsonSerializer.Deserialize<Vote>(serialized, JsonDefaultSettings.Options);
			
			Assert.IsNotNull(vote2);
			Assert.AreEqual(questionId, vote2.QuestionId);
		}

		[Test]
		public void Time()
		{
			var serialized = JsonSerializer.Serialize(vote, JsonDefaultSettings.Options);
			var vote2 = JsonSerializer.Deserialize<Vote>(serialized, JsonDefaultSettings.Options);

			Assert.IsNotNull(vote2);
			Assert.AreEqual(time, vote2.Time);
		}

		[Test]
		public void Choice()
		{
			var serialized = JsonSerializer.Serialize(vote, JsonDefaultSettings.Options);
			var vote2 = JsonSerializer.Deserialize<Vote>(serialized, JsonDefaultSettings.Options);

			Assert.IsNotNull(vote2);
			Assert.AreEqual(choiceId, vote2.ChoiceId);
		}

		[Test]
		public void PublicKey()
		{
			var serialized = JsonSerializer.Serialize(vote, JsonDefaultSettings.Options);
			var vote2 = JsonSerializer.Deserialize<Vote>(serialized, JsonDefaultSettings.Options);

			Assert.IsNotNull(vote2);

			var publicKey = Base58.Decode(publicKeyEncoded);
			Assert.IsTrue(publicKey.SequenceEqual(vote2.PublicKey));
		}

		[Test]
		public void Signature()
		{
			var serialized = JsonSerializer.Serialize(vote, JsonDefaultSettings.Options);
			var vote2 = JsonSerializer.Deserialize<Vote>(serialized, JsonDefaultSettings.Options);

			Assert.IsNotNull(vote2);

			var signature = Base58.Decode(signatureEncoded);
			Assert.IsTrue(signature.SequenceEqual(vote2.Signature));
		}
	}
}