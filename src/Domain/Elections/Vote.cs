using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Utils;

namespace Domain.Elections
{
	public class Vote : BlockItem
	{
		private const int QuestionOffSet = 0;
		private const int QuestionSize = 16;
		private const int ChoiceOffSet = QuestionOffSet + QuestionSize;
		private const int ChoiceSize = 16;
		private const int TimeOffSet = ChoiceOffSet + ChoiceSize;
		private const int TimeSize = 8;

		public Guid QuestionId { get; set; }
		public long Time { get; set; }

		public Guid ChoiceId { get; set; }

		public override string GetKey()
		{
			return BuildKey(QuestionId, Base58.Encode(PublicKey));
		}

		public override byte[] GetData()
		{
			var data = new byte[QuestionSize+ ChoiceSize+ TimeSize];

			Buffer.BlockCopy(QuestionId.ToOrderByteArray(), 0, data, QuestionOffSet, QuestionSize);
			Buffer.BlockCopy(ChoiceId.ToOrderByteArray(), 0, data, ChoiceOffSet, ChoiceSize);

			var time = BitConverter.GetBytes(Time);
			Array.Reverse(time);
			Buffer.BlockCopy(time, 0, data, TimeOffSet, TimeSize);

			return data;
		}

		public static string BuildKey(Guid questionId, string publicKey)
		{
			return $"{questionId:n}:{publicKey}";
		}

		public override bool IsValid(IList<Block> chain)
		{
			if (!base.IsValid(chain)) return false;

			var members = chain.SelectMany(b => b.Members.Where(m => m.Address.SequenceEqual(PublicKey))).ToArray();
			if (members.Length == 0)
			{
				// TODO: validar que el miembro sea de la organización ¿y que esté en el padrón o sea miembro desde antes del inicio de la votación?
				Messages.Add($"La persona [{Base58.Encode(PublicKey)}] no es miembro de esta comunidad");
				return false;
			}
				
			var member = members[0];

			// TODO: ¿que pasa si los dos votos están entrando en el mismo bloque?
			var previous = chain.Any(b => b.Votes.Any(v => v.QuestionId.Equals(QuestionId) && v.PublicKey.SequenceEqual(PublicKey)));
			if (previous)
			{
				Messages.Add($"La persona [{Base58.Encode(PublicKey)}] ya votó");
				return false;
			}

			return true;
		}
	}
}