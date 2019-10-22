using System;
using System.Text;
using Domain.Crypto;
using Domain.Utils;
using NUnit.Framework;

namespace Tests.Signatures
{
	public class Crypto_secp256_k1_tests
	{
		private readonly ICryptoService crypto = new CryptoSecp256k1();

		[Test]
		public void Generate_keys()
		{
			var pair = crypto.GeneratePair();

			var privateKey = pair.PrivateKey;
			var publicKey = pair.PublicKey;

			Console.WriteLine("Generated private key: " + Base58.Encode(privateKey));
			Console.WriteLine("Generated public key: " + Base58.Encode(publicKey));
		}
		
		[Test]
		public void Basic()
		{
			// Step 1
			var message = Encoding.UTF8.GetBytes("Hello World!");

			// Step 2
			var keys = crypto.GeneratePair();

			var publicKey = keys.PublicKey;

			Console.WriteLine($"Key length: {keys.PrivateKey.Length}");
			Console.WriteLine($"public key: {Base58.Encode(publicKey)}");

			// Step 3
			var signature = crypto.GetSignature(keys.PrivateKey, message);
			Console.WriteLine($"signature: {Base58.Encode(signature)}");

			// Step 4
			Assert.IsTrue(crypto.VerifySignature(message, publicKey, signature));
		}

		[Test]
		public void Basic2()
		{
			// Step 1
			var message = Encoding.UTF8.GetBytes("Hello World!");

			// Step 2
			var keys = crypto.GeneratePair();

			Console.WriteLine($"Key length: {keys.PrivateKey.Length}");
			Console.WriteLine($"public key: {Base58.Encode(keys.PublicKey)}");

			// Step 3
			var signature = crypto.GetSignature(keys.PrivateKey, message);
			Console.WriteLine($"signature: {Base58.Encode(signature)}");

			// Step 4
			Assert.IsTrue(crypto.VerifySignature(message, keys.PublicKey, signature));
		}
	}
}