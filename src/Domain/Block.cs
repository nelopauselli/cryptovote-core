using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Domain.Elections;

namespace Domain
{
	public class Block
	{
		public Block(byte[] previousHash, int blockNumber)
		{
			PreviousHash = previousHash;
			BlockNumber = blockNumber;
			Documents = new List<Document>();
			Communities = new List<Community>();
			Members = new List<Member>();
			Issues = new List<Issue>();
			Urns = new List<Urn>();
			Fiscals = new List<Fiscal>();
			Votes = new List<Vote>();
			Recounts = new List<Recount>();
			Recognitions = new List<Recognition>();
		}

		public int BlockNumber { get; set; }
		public byte Dificulty { get; set; }

		public IList<Document> Documents { get; }
		public IList<Community> Communities { get; }
		public IList<Member> Members { get; }
		public IList<Issue> Issues { get; }
		public IList<Urn> Urns { get; }
		public IList<Fiscal> Fiscals { get; }
		public IList<Vote> Votes { get; }
		public IList<Recount> Recounts { get; }
		public IList<Recognition> Recognitions { get; }

		public int Nonce { get; set; }
		public byte[] MinerPublicKey { get; set; }
		public byte[] Hash { get; set; }
		public byte[] PreviousHash { get; }

		public BlockItem[] GetTransactions()
		{
			var transactions = new List<BlockItem>();
			transactions.AddRange(Documents);
			transactions.AddRange(Communities);
			transactions.AddRange(Members);
			transactions.AddRange(Issues);
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
			return Documents.Any() || Communities.Any() || Members.Any() || Issues.Any() || Urns.Any() || Fiscals.Any() || Votes.Any() || Recounts.Any() || Recognitions.Any();
		}

		public override string ToString()
		{
			return BitConverter.ToString(Hash);
		}
	}
}