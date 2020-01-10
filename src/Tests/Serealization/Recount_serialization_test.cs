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
	public class Recount_serialization_test
	{
		const string raw = "{\"Id\":\"ad3dca890a6640c28a6a4d632d1c8293\",\"UrnId\":\"7830df01696a4505843310e6dcb96583\",\"Results\":[{\"ChoiceId\":\"dd40ec78dd674bcb8ba352f8e1fa4050\",\"Votes\":1},{\"ChoiceId\":\"baab4edb57584b97adabbf9082f90ab9\",\"Votes\":2},{\"ChoiceId\":\"9f61d4bbe9b24e1198ec181bf8049c07\",\"Votes\":3},{\"ChoiceId\":\"ae35ce85ca5341659b0ccbf530b939ef\",\"Votes\":4},{\"ChoiceId\":\"a77ce381786c4500a70df1a1731a8057\",\"Votes\":5}],\"PublicKey\":\"MXPqG6YZJu8yCZxAiusyCREPK3N9QTSBJUayVo4YHMF2Gq5kRQhHFyPGHf258umTT8MEm6E3MGvUBNsUtu2XAg6a\",\"Signature\":\"AN1rKqnEvaUPYk4xgkbnhBcqAHn9duTdejLH6SosqgYWM6xCVtJcgo7UCT4xZKJjeGKSSN5XPhKuf73wKELeXwNuHSdFYDMFU\"}";

		[Test]
		public void Deserealize()
		{
			var recount = JsonSerializer.Deserialize<Recount>(raw, JsonDefaultSettings.Options);
			Assert.AreEqual(Guid.Parse("ad3dca89-0a66-40c2-8a6a-4d632d1c8293"),recount.Id);
			Assert.AreEqual(Guid.Parse("7830df01-696a-4505-8433-10e6dcb96583"),recount.UrnId);
			Assert.IsNotNull(recount.Results);
			Assert.AreEqual(5, recount.Results.Length);

			Assert.AreEqual(Guid.Parse("dd40ec78-dd67-4bcb-8ba3-52f8e1fa4050"), recount.Results[0].ChoiceId);
			Assert.AreEqual(1, recount.Results[0].Votes);
			Assert.AreEqual(Guid.Parse("baab4edb-5758-4b97-adab-bf9082f90ab9"), recount.Results[1].ChoiceId);
			Assert.AreEqual(2, recount.Results[1].Votes);

			Assert.AreEqual("MXPqG6YZJu8yCZxAiusyCREPK3N9QTSBJUayVo4YHMF2Gq5kRQhHFyPGHf258umTT8MEm6E3MGvUBNsUtu2XAg6a", Base58.Encode(recount.PublicKey));
			Assert.AreEqual("AN1rKqnEvaUPYk4xgkbnhBcqAHn9duTdejLH6SosqgYWM6xCVtJcgo7UCT4xZKJjeGKSSN5XPhKuf73wKELeXwNuHSdFYDMFU", Base58.Encode(recount.Signature));
		}

		[Test]
		public void Data()
		{
			var recount = JsonSerializer.Deserialize<Recount>(raw);

			var dataRaw = "2PUvrcNWrVPFZ8TMZhjsX61ef85RcEqG3nQSor17XbtMZbkd9Tsj9SATPRK2KyFFGHsSMGjD8D7H2bqLgvmuP1sZjeE78Yx3qTttmjwb4sVCsF2uKkhAXF76T84ZHFg6fSRcoM88ABrSyYDDsuzUx6KM5rLSttnY3KDbio5GT2Bsg2daLrojJ";
			var data = Base58.Decode(dataRaw);
			
			Console.WriteLine(BitConverter.ToString(data));
			Console.WriteLine(BitConverter.ToString(recount.GetData()));
			
			Assert.IsTrue(data.SequenceEqual(recount.GetData()));
		}
	}
}