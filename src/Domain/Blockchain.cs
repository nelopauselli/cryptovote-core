using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Domain.Elections;

namespace Domain
{
	public class Blockchain
	{
		private readonly Miner miner;
		private readonly IBlockBuilder blockBuilder;
		private readonly byte dificulty;
		private readonly IList<Block> trunk;
		private readonly IList<IList<Block>> branches = new List<IList<Block>>();

		public Blockchain(Miner miner, IBlockBuilder blockBuilder, byte dificulty)
		{
			this.miner = miner ?? throw new ArgumentNullException(nameof(miner));
			this.blockBuilder = blockBuilder ?? throw new ArgumentNullException(nameof(blockBuilder));

			this.dificulty = dificulty;
			trunk = new List<Block>();
		}

		public IEnumerable<Block> Trunk => trunk;
		public int BranchesCount => branches.Count;
		public Block Last { get; private set; }

		public Block MineNextBlock(BlockItem[] transactions)
		{
			var block = blockBuilder.BuildNextBlock(transactions.Where(i => i.IsValid(trunk)).ToArray(), Last, dificulty);
			if (!block.IsNotEmpty()) return null;

			if (!miner.Mine(block, dificulty)) 
				return null;

			// Verificando que nadie nos gan� al minar y que no paramos a tiempo nuestro minado
			if (Last != null && !Last.Hash.SequenceEqual(block.PreviousHash))
				return null;

			AddBlock(block);
			return block;

		}

		public void StopMine()
		{
			miner.Stop();
		}

		public Block GetBlock(int index)
		{
			return trunk[index];
		}

		public Block GetBlock(byte[] hash)
		{
			Block block = trunk.FirstOrDefault(b => b.Hash.SequenceEqual(hash));
			if (block != null) return block;

			foreach (var branch in branches)
			{
				block = branch.FirstOrDefault(b => b.Hash.SequenceEqual(hash));
				if (block != null) return block;
			}

			return null;
		}

		public Block GetNextTo(byte[] hash)
		{
			return trunk.FirstOrDefault(b => b.PreviousHash.SequenceEqual(hash));
		}


		public bool IsValid()
		{
			byte[] previousHash = null;
			for (var index = 0; index < trunk.Count; index++)
			{
				var block = trunk[index];
				if (block.Hash == null) return false;

				if (!block.IsValid())
					return false;

				if (index > 0)
				{
					if (block.PreviousHash != previousHash)
						return false;
				}

				previousHash = block.Hash;
			}

			return true;
		}

		public void AddBlock(Block block)
		{
			// TODO: verificar validez del bloque

			if (Last == null || Last.Hash.SequenceEqual(block.PreviousHash))
			{
				Last = block;
				trunk.Add(block);

				SearchBranchAfterLast();
			}
			else
			{
				var branch = AddBlockToBranch(block);
				if (branch == null)
				{
					branch = new List<Block> {block};
					branches.Add(branch);
				}
				else
				{
					PivotToLongerChain(branch);
				}
			}
		}

		private void PivotToLongerChain(IList<Block> current)
		{
			var branchStart = current[0];
			var pivot = trunk.SingleOrDefault(b => b.Hash.SequenceEqual(branchStart.PreviousHash));
			if (pivot != null)
			{
				if (pivot.BlockNumber + 1 + current.Count > trunk.Count)
				{
					// Creo un nuevo branch con los nodos del trunk
					var branch = new List<Block>();
					var old = GetNextTo(pivot.Hash);
					while (old != null)
					{
						branch.Add(old);
						old = GetNextTo(old.Hash);
					}

					branches.Add(branch);

					// Quito del trunk los nodos del nuevo branch
					foreach (var moved in branch)
						trunk.Remove(moved);

					// Muevo el branch al trunk
					foreach (var b in current)
						trunk.Add(b);

					Last = trunk[trunk.Count - 1];

					branches.Remove(current);
				}
			}
		}

		private IList<Block> AddBlockToBranch(Block block)
		{
			IList<Block> current = null;
			foreach (var branch in branches)
			{
				if (block.Hash.SequenceEqual(branch[0].PreviousHash))
				{
					branch.Insert(0, block);
					current = branch;

					break;
				}
				else if (block.PreviousHash.SequenceEqual(branch[branch.Count - 1].Hash))
				{
					branch.Add(block);
					current = branch;

					break;
				}
			}

			return current;
		}

		private void SearchBranchAfterLast()
		{
			foreach (var branch in branches)
			{
				if (Last.Hash.SequenceEqual(branch[0].PreviousHash))
				{
					foreach (var b in branch)
						trunk.Add(b);

					branches.Remove(branch);
					Last = trunk[trunk.Count - 1];

					break;
				}
			}
		}

		public Block GetBlock(int index, int branchIndex)
		{
			if (branches.Count <= branchIndex) return null;
			var branch = branches[branchIndex];

			if (branch.Count <= index) return null;
			return branch[index];
		}

		public long GetRecountFor(Guid questionId, Guid choiceId)
		{
			Question question = null;
			var urns = new List<Guid>();
			var recounts = new List<Recount>();

			foreach (var block in trunk)
			{
				if (question == null)
				{
					foreach (var i in block.Questions)
					{
						if (i.Id == questionId)
						{
							question = i;
							break;
						}
					}
				}

				if (question != null)
				{
					foreach (var urn in block.Urns)
					{
						if (urn.QuestionId == questionId)
						{
							urns.Add(urn.Id);
						}
					}

					foreach (var recount in block.Recounts)
					{
						if (urns.Contains(recount.UrnId))
							recounts.Add(recount);
					}
				}
			}

			var recountByUrn = recounts.GroupBy(r => r.UrnId);

			long total = 0;
			foreach (var group in recountByUrn)
			{
				var results = group.SelectMany(g => g.Results).Where(r => r.ChoiceId == choiceId).ToArray();

				var votes = results[0].Votes;
				foreach (var result in results)
				{
					if (result.Votes != votes)
					{
						votes = 0;
						break;
					}
				}

				total += votes;
			}

			return total;
		}

		public Recount GetRecount(Guid urnId)
		{
			foreach (var block in trunk)
			{
				var recount = block.Recounts.SingleOrDefault(u => u.UrnId == urnId);
				if (recount != null) return recount;
			}

			return null;
		}

		public int CountRecognitions(Guid urnId)
		{
			Urn urn = null;
			Recount recount = null;
			var recognitions = 0;
			var choices = new List<Guid>();

			foreach (var block in trunk)
			{
				if (urn == null)
					urn = block.Urns.SingleOrDefault(u => u.Id == urnId);

				if (recount == null)
					recount = block.Recounts.SingleOrDefault(u => u.UrnId == urnId);

				if (urn != null && recount != null)
				{
					foreach (var recognition in block.Recognitions)
					{
						if (recognition.UrnId == urnId)
						{
							if (recognition.Content.SequenceEqual(recount.GetData()))
							{
								if(urn.Authorities.Any(a=>a.SequenceEqual(recognition.PublicKey)))
								{
									recognitions++;
								}
								else
								{
									var choiceId = GetChoiceFromSigner(urn.QuestionId, recognition.PublicKey);
									if (choiceId.HasValue && !choices.Contains(choiceId.Value))
									{
										choices.Add(choiceId.Value);
										recognitions++;
									}
								}
							}
							else
								Trace.TraceWarning($"El reconocimiento {recognition.Id} no es v�lido");
						}
					}
				}
			}

			if (recount == null)
				Trace.TraceWarning($"No se encontr� la urna {urnId} en la blockchain");

			return recognitions;
		}

		public Guid? GetChoiceFromSigner(Guid questionId, byte[] publicKey)
		{
			Question question = null;
			foreach (var block in trunk)
			{
				if (question == null)
					question = block.Questions.SingleOrDefault(i => i.Id == questionId);

				if (question != null)
				{
					var fiscal = block.Fiscals.SingleOrDefault(f => f.QuestionId == questionId && f.Address.SequenceEqual(publicKey));
					if (fiscal != null)
					{
						foreach (var choice in question.Choices)
						{
							if (fiscal.PublicKey.SequenceEqual(choice.GuardianAddress))
								return fiscal.ChoiceId;
						}

						return null;
					}
				}
			}

			return null;
		}

		public QuestionResult GetResult(Guid questionId)
		{
			QuestionResult result = null;
			var urns = new List<Guid>();

			foreach (var block in trunk)
			{
				if (result == null)
				{
					var question = block.Questions.SingleOrDefault(i => i.Id == questionId);
					if (question != null)
						result = new QuestionResult
						{
							QuestionId = question.Id,
							Type = question.Type,
							Choices = question.Choices.Select(c => new ChoiceResult
							{
								ChoiceId = c.Id,
								Text = c.Text,
								Color = c.Color,
								Votes = 0
							}).ToArray()
						};
				}

				if (result != null)
				{
					if (result.Type == QuestionType.DirectVote)
					{
						var votesGroups = block.Votes.Where(v => v.QuestionId == result.QuestionId).GroupBy(v => v.ChoiceId);
						foreach (var votes in votesGroups)
						{
							var choice = result.Choices.SingleOrDefault(c => c.ChoiceId == votes.Key);
							if (choice != null)
								choice.Votes += votes.Count();
						}
					}
					else if (result.Type == QuestionType.Recount)
					{
						urns.AddRange(block.Urns.Where(u => u.QuestionId == questionId).Select(u => u.Id));

						var recounts = block.Recounts.Where(r => urns.Contains(r.UrnId));
						foreach (var recount in recounts)
						{
							foreach (var recountResult in recount.Results)
							{
								var choice = result.Choices.SingleOrDefault(c => c.ChoiceId == recountResult.ChoiceId);
								if (choice != null)
									choice.Votes += recountResult.Votes;
							}
						}
					}
				}
			}

			return result;
		}
	}
}