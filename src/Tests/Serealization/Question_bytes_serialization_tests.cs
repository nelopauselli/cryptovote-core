using System;
using System.Linq;
using System.Text;
using Domain.Elections;
using Domain.Utils;
using NUnit.Framework;

namespace Tests.Serealization
{
	public class Question_bytes_serialization_tests
	{
		private readonly Question question;

		public Question_bytes_serialization_tests()
		{
			question = new Question
			{
				CommunityId = Guid.NewGuid(),
				Id = Guid.NewGuid(),
				EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
				Type = 1,
				Name = "Test Question",
				Choices = new[]
				{
					new Choice {Id = Guid.NewGuid(), Color = 0x673ab7, Text = "Opción 1"},
					new Choice {Id = Guid.NewGuid(), Color = 0xe91e63, Text = "Opción 2"},
					new Choice {Id = Guid.NewGuid(), Color = 0xff5722, Text = "Opción 3"},
					new Choice {Id = Guid.NewGuid(), Color = 0x4caf50, Text = "Opción 4"}
				}
			};
		}

		[Test]
		public void QuestionId_in_data()
		{
			var data = question.GetData();

			var buffer = new byte[16];
			Array.Copy(data, 0, buffer, 0, 16);
			Assert.IsTrue(question.Id.ToOrderByteArray().SequenceEqual(buffer));
		}

		[Test]
		public void CommunityId_in_data()
		{
			var data = question.GetData();

			var buffer = new byte[16];
			Array.Copy(data, 16, buffer, 0, 16);
			Assert.IsTrue(question.CommunityId.ToOrderByteArray().SequenceEqual(buffer));
		}

		[Test]
		public void Name_in_data()
		{
			var data = question.GetData();

			var name = Encoding.UTF8.GetString(data, 32, question.Name.Length).Trim();
			Assert.AreEqual(question.Name, name);
		}

		[Test]
		public void EndTime_in_data()
		{
			var data = question.GetData();

			var buffer = new byte[8];
			Array.Copy(data, 16 + 16 + question.Name.Length + 1, buffer, 0, 8);

			Array.Reverse(buffer);
			Assert.AreEqual(question.EndTime, BitConverter.ToInt64(buffer));
		}

		[Test]
		public void Type_in_data()
		{
			var data = question.GetData();

			var offset = 16 + 16 + question.Name.Length;

			Assert.AreEqual(1, data[offset]);
		}

		[Test]
		public void Choice_in_data()
		{
			var data = question.GetData();
			Console.WriteLine(BitConverter.ToString(data));

			var buffer = new byte[16 + 4 + 9];
			Array.Copy(data, 16 + 16 + question.Name.Length + 1 + 8, buffer, 0, 29);

			var choice = question.Choices[0];
			Console.WriteLine(BitConverter.ToString(choice.Id.ToOrderByteArray()));
			Console.WriteLine(BitConverter.ToString(buffer.Take(16).ToArray()));
			Assert.IsTrue(choice.Id.ToOrderByteArray().SequenceEqual(buffer.Take(16).ToArray()));
			Assert.AreEqual(0x00, buffer[16]);
			Assert.AreEqual(0x67, buffer[16 + 1]);
			Assert.AreEqual(0x3a, buffer[16 + 2]);
			Assert.AreEqual(0xb7, buffer[16 + 3]);

			var text = Encoding.UTF8.GetString(buffer, 16 + 4, 9).Trim();
			Assert.AreEqual("Opción 1", text);
		}
	}
}