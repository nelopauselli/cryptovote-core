using System.Linq;
using Domain;
using Domain.Crypto;
using Domain.Scrutiny;
using NUnit.Framework;

namespace Tests
{
	public class Consensus_tests
	{
		private Miner miner;

		[OneTimeSetUp]
		public void InitKeys()
		{
			var minerKeys = CryptoService.Instance.GeneratePair();
			miner = new Miner(minerKeys.PublicKey);
		}

		[Test]
		public void Trunk()
		{
			var blockchain = new Blockchain(miner, new BlockBuilder(), 1);

			var genesis = new Block(new byte[] { }, 0) {Hash = new byte[] {1, 5, 2}};
			blockchain.AddBlock(genesis);

			var block1 = new Block(genesis.Hash, 1) {Hash = new byte[] {2, 8, 7}};
			blockchain.AddBlock(block1);

			Assert.AreEqual(2, blockchain.Trunk.Count());
		}

		[Test]
		public void Branch()
		{
			var blockchain = new Blockchain(miner, new BlockBuilder(), 1);

			var genesis = new Block(new byte[] { }, 0) {Hash = new byte[] {1, 5, 2}};
			blockchain.AddBlock(genesis);

			Assert.AreEqual(1, blockchain.Trunk.Count());
			Assert.AreEqual(0, blockchain.BranchesCount);

			var block1A = new Block(genesis.Hash, 1) {Hash = new byte[] {2, 8, 7}};
			blockchain.AddBlock(block1A);

			Assert.AreEqual(2, blockchain.Trunk.Count());
			Assert.AreEqual(0, blockchain.BranchesCount);

			var block1B = new Block(genesis.Hash, 1) {Hash = new byte[] {2, 1, 7}};
			blockchain.AddBlock(block1B);

			Assert.AreEqual(2, blockchain.Trunk.Count());
			Assert.AreEqual(1, blockchain.BranchesCount);

			var block1C = new Block(genesis.Hash, 1) {Hash = new byte[] {2, 1, 2}};
			blockchain.AddBlock(block1C);

			Assert.AreEqual(2, blockchain.Trunk.Count());
			Assert.AreEqual(2, blockchain.BranchesCount);
		}

		[Test]
		public void Move_branch_to_main()
		{
			var blockchain = new Blockchain(miner, new BlockBuilder(), 1);

			var genesis = new Block(new byte[] { }, 0) {Hash = new byte[] {1, 5, 2}};
			blockchain.AddBlock(genesis);

			Assert.AreEqual(genesis, blockchain.Last);
			Assert.AreEqual(0, blockchain.BranchesCount);

			var block1A = new Block(genesis.Hash, 1) {Hash = new byte[] {2, 8, 7}};
			blockchain.AddBlock(block1A);

			Assert.AreEqual(block1A, blockchain.Last);
			Assert.AreEqual(0, blockchain.BranchesCount);

			var block1B = new Block(genesis.Hash, 1) {Hash = new byte[] {2, 1, 7}};
			blockchain.AddBlock(block1B);

			Assert.AreEqual(block1A, blockchain.Last);
			Assert.AreEqual(1, blockchain.BranchesCount);

			var block2B = new Block(block1B.Hash, 2) {Hash = new byte[] {2, 1, 2}};
			blockchain.AddBlock(block2B);

			Assert.AreEqual(block2B, blockchain.Last);

			Assert.AreEqual(3, blockchain.Trunk.Count());
			Assert.IsTrue(genesis.Hash.SequenceEqual(blockchain.GetBlock(0).Hash));
			Assert.IsTrue(block1B.Hash.SequenceEqual(blockchain.GetBlock(1).Hash));
			Assert.IsTrue(block2B.Hash.SequenceEqual(blockchain.GetBlock(2).Hash));

			Assert.AreEqual(1, blockchain.BranchesCount);
		}

		[Test]
		public void Unsorted_blocks_in_trunk()
		{
			var blockchain = new Blockchain(miner, new BlockBuilder(), 1);

			var genesis = new Block(new byte[] { }, 0) {Hash = new byte[] {1, 5, 2}};
			blockchain.AddBlock(genesis);

			Assert.AreEqual(genesis, blockchain.Last);
			Assert.AreEqual(0, blockchain.BranchesCount);

			var block1A = new Block(genesis.Hash, 1) {Hash = new byte[] {2, 8, 7}};
			var block2A = new Block(block1A.Hash, 2) {Hash = new byte[] {2, 8, 4}};

			blockchain.AddBlock(block2A);

			Assert.AreEqual(genesis, blockchain.Last);
			Assert.AreEqual(1, blockchain.BranchesCount);

			blockchain.AddBlock(block1A);

			Assert.AreEqual(block2A, blockchain.Last);

			Assert.AreEqual(3, blockchain.Trunk.Count());
			Assert.IsTrue(genesis.Hash.SequenceEqual(blockchain.GetBlock(0).Hash));
			Assert.IsTrue(block1A.Hash.SequenceEqual(blockchain.GetBlock(1).Hash));
			Assert.IsTrue(block2A.Hash.SequenceEqual(blockchain.GetBlock(2).Hash));

			Assert.AreEqual(0, blockchain.BranchesCount);
		}

		[Test]
		public void Unsorted_blocks_in_branch()
		{
			var blockchain = new Blockchain(miner, new BlockBuilder(), 1);

			var genesis = new Block(new byte[] { }, 0) {Hash = new byte[] {1, 5, 2}};
			blockchain.AddBlock(genesis);

			Assert.AreEqual(genesis, blockchain.Last);
			Assert.AreEqual(0, blockchain.BranchesCount);

			var block1A = new Block(genesis.Hash, 1) {Hash = new byte[] {2, 8, 7}};
			blockchain.AddBlock(block1A);

			Assert.AreEqual(block1A, blockchain.Last);
			Assert.AreEqual(0, blockchain.BranchesCount);

			var block1B = new Block(genesis.Hash, 2) {Hash = new byte[] {2, 8, 4}};
			var block2B = new Block(block1B.Hash, 3) {Hash = new byte[] {6, 4, 1}};

			blockchain.AddBlock(block2B);

			Assert.AreEqual(block1A, blockchain.Last);
			Assert.AreEqual(1, blockchain.BranchesCount);

			blockchain.AddBlock(block1B);

			Assert.AreEqual(block2B, blockchain.Last);

			Assert.AreEqual(3, blockchain.Trunk.Count());
			Assert.IsTrue(genesis.Hash.SequenceEqual(blockchain.GetBlock(0).Hash));
			Assert.IsTrue(block1B.Hash.SequenceEqual(blockchain.GetBlock(1).Hash));
			Assert.IsTrue(block2B.Hash.SequenceEqual(blockchain.GetBlock(2).Hash));

			Assert.AreEqual(1, blockchain.BranchesCount);

			Assert.AreEqual(block1A, blockchain.GetBlock(0, 0));
		}
	}
}