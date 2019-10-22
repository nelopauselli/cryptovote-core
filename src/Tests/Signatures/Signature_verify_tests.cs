using System;
using System.Text;
using Domain.Crypto;
using Domain.Utils;
using NUnit.Framework;

namespace Tests.Signatures
{
	public class Signature_verify_tests
	{
		[Test]
		public void VerifyHolaMundo()
		{
			var service = new CryptoSecp256k1();

			byte[] message = Encoding.UTF8.GetBytes("Hola Mundo!");
			var publicKey = Base58.Decode("ReXPQW5VXp3sgKe1fjpUwpXFAThPjDE5erDLYmjynEMCBqGaFo9bPoDy1GyFw4HdKM6ezKY681CNVuWJYjM9tQSr");
			var signature = Base58.Decode("AN1rKvtCZ2qZniW44srYMYCYUMCwXNeXUWFdA45NbF4KtUZLmWSnpfQc5pKurzqvLSRpQKQoDh2Wuw91dVJioUhLREVm3hjzy");

			Assert.IsTrue(service.VerifySignature(message, publicKey, signature));
		}

		[Test]
		public void SignAndVerifyHolaMundo()
		{
			var service = new CryptoSecp256k1();

			byte[] message = Encoding.UTF8.GetBytes("Hola Mundo!");
			var privateKey = Base58.Decode("2dx4DnPA65jFDx3rXFUkq8JwmfvkDCYaDpjF2X66Hxfi");
			var publicKey = service.GetPublicKeyFromPrivateKeyEx(privateKey);
			Console.WriteLine($"Public Key: {Base58.Encode(publicKey)}");

			var sign = service.GetSignature(privateKey, message);
			Console.WriteLine($"Signature: {Base58.Encode(sign)}");


			Assert.IsTrue(service.VerifySignature(message, publicKey, sign));
		}
	}
}