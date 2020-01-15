using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Utils;

namespace Domain.Elections
{
	public class Question : BlockItem
	{
		public Guid Id { get; set; }
		public Guid CommunityId { get; set; }
		public string Name { get; set; }
		public byte Type { get; set; }
		public long EndTime { get; set; }
		public Choice[] Choices { get; set; } = Array.Empty<Choice>();

		public override byte[] GetData()
		{
			var choices = new List<byte[]>();
			foreach (var choice in Choices ?? Array.Empty<Choice>())
				choices.Add(choice.GetData());

			var nameAsBytes = Encoding.UTF8.GetBytes(Name);

			var data = new byte[16 + 16 + nameAsBytes.Length + 1 + 8 + 8 + choices.Sum(c => c.Length)];
			var offset = 0;
			Buffer.BlockCopy(Id.ToOrderByteArray(), 0, data, offset, 16);
			offset += 16;
			Buffer.BlockCopy(CommunityId.ToOrderByteArray(), 0, data, offset, 16);
			offset += 16;
			Buffer.BlockCopy(nameAsBytes, 0, data, offset, nameAsBytes.Length);
			offset += nameAsBytes.Length;

			data[offset] = Type;
			offset += 1;

			var time = BitConverter.GetBytes(EndTime);
			Array.Reverse(time);
			Buffer.BlockCopy(time, 0, data, offset, 8);
			offset += 8;

			foreach (var choice in choices)
			{
				Buffer.BlockCopy(choice, 0, data, offset, choice.Length);
				offset += choice.Length;
			}

			return data;
		}

		public override string GetKey()
		{
			return BuildKey(CommunityId, Id);
		}

		public static string BuildKey(Guid communityId, Guid questionId)
		{
			return $"{communityId:n}:{questionId:n}";
		}

		public override bool IsValid(IList<Block> chain)
		{
			if (!base.IsValid(chain)) return false;

			var registered = chain.Where(b => b.Communities != null).Any(b => b.Communities.Any(t => t.Id == CommunityId && t.Address.SequenceEqual(PublicKey)));
			if (!registered)
				Messages.Add($"La PublicKey {PublicKey.ByteArrayToHexString()} no es dueña de la comunidad {CommunityId:n}");

			return registered;
		}
	}

	public static class QuestionType
	{
		public static byte DirectVote = 1;
		public static byte Recount = 2;
	}
}