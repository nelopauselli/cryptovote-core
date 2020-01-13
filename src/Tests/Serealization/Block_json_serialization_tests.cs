using System;
using System.Text.Json;
using Domain;
using Domain.Converters;
using Domain.Elections;
using NUnit.Framework;

namespace Tests.Serealization
{
	public class Block_json_serialization_tests
	{
		[Test]
		public void Community()
		{
			byte[] previousHash = {1, 2, 3, 4, 5};
			
			var community = new Community
			{
				Id = new Guid("4814e8f885f74230b04d2daa4e2d88a4"),
				Name = "Crypto Vote",
				CreateAt = DateTimeOffset.FromUnixTimeMilliseconds(1551715934000)
			};
			var question = new Question
			{
				Id = Guid.NewGuid(),
				CommunityId = community.Id,
				Name = "Raspberry",
				Choices = new[] {new Choice {Text = "Si"}, new Choice {Text = "No"}}
			};
			
			var block1 = new Block(previousHash, 7);
			block1.Communities.Add(community);
			block1.Questions.Add(question);

			var json = JsonSerializer.Serialize(block1, JsonDefaultSettings.Options);
			Console.WriteLine(json);

			var block2 = JsonSerializer.Deserialize<Block>(json, JsonDefaultSettings.Options);
			Assert.AreEqual(7, block2.BlockNumber);
			Assert.AreEqual(1, block2.Communities?.Count);
			Assert.AreEqual(1, block2.Questions?.Count);
			Assert.AreEqual(2, block2.GetTransactions().Length);
		}
	}
}