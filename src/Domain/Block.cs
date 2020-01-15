using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Domain.Elections;

namespace Domain
{
	public class Block
	{
		public Block()
		{
		}

		public Block(byte[] previousHash, int blockNumber)
		{
			PreviousHash = previousHash;
			BlockNumber = blockNumber;
		}

		public int BlockNumber { get; set; }
		public byte Dificulty { get; set; }

		public List<Document> Documents { get; set; } = new List<Document>();
		public List<Community> Communities { get; set; } = new List<Community>();
		public List<Member> Members { get; set; } = new List<Member>();
		public List<Question> Questions { get; set; } = new List<Question>();
		public List<Urn> Urns { get; set; } = new List<Urn>();
		public List<Fiscal> Fiscals { get; set; } = new List<Fiscal>();
		public List<Vote> Votes { get; set; } = new List<Vote>();
		public List<Recount> Recounts { get; set; } = new List<Recount>();
		public List<Recognition> Recognitions { get; set; }  = new List<Recognition>();

		public int Nonce { get; set; }
		public byte[] MinerPublicKey { get; set; }
		public byte[] Hash { get; set; }
		public byte[] PreviousHash { get; set; } = Array.Empty<byte>();

		public BlockItem[] GetTransactions()
		{
			var transactions = new List<BlockItem>();
			transactions.AddRange(Documents);
			transactions.AddRange(Communities);
			transactions.AddRange(Members);
			transactions.AddRange(Questions);
			transactions.AddRange(Urns);
			transactions.AddRange(Fiscals);
			transactions.AddRange(Recounts);
			transactions.AddRange(Recognitions);
			transactions.AddRange(Votes);

			return transactions.ToArray();
		}

		public byte[] GetData()
		{
			var transactions = GetTransactions();

			var data = new byte[32 * transactions.Length + MinerPublicKey.Length + 4+1];

			var offset = 0;
			foreach (var transaction in transactions)
			{
				byte[] hash;

				var td = transaction.GetData();
				using (var sha256 = new SHA256Managed())
				{
					sha256.Initialize();
					hash = sha256.ComputeHash(td);
				}

				Buffer.BlockCopy(hash, 0, data, offset, 32);
				offset += 32;
			}

			Buffer.BlockCopy(MinerPublicKey, 0, data, offset, MinerPublicKey.Length);
			offset += MinerPublicKey.Length;

			Buffer.BlockCopy(BitConverter.GetBytes(Nonce), 0, data, offset, 4);
			offset += 4;

			data[offset]=Dificulty;

			return data;
		}

		public bool IsValid()
		{
			if (Hash == null || Hash.SequenceEqual(Array.Empty<byte>()))
				return false;

			if (Dificulty == 0) 
				return false;

			for (var index = 0; index < Dificulty; index++)
				if (Hash[index] != 0)
					return false;

			return true;
		}

		public bool IsNotEmpty()
		{
			return Documents.Any() || Communities.Any() || Members.Any() || Questions.Any() || Urns.Any() || Fiscals.Any() || Votes.Any() || Recounts.Any() || Recognitions.Any();
		}

		public override string ToString()
		{
			return BitConverter.ToString(Hash);
		}
	}
}