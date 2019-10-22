using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Domain.Crypto
{
	public class CryptoSecp256k1 : ICryptoService
	{
		readonly X9ECParameters ecParams;
		private readonly ECDomainParameters domain;

		public CryptoSecp256k1()
		{
			ecParams = SecNamedCurves.GetByName("secp256k1");
			domain = new ECDomainParameters(ecParams.Curve, ecParams.G, ecParams.N, ecParams.H, ecParams.GetSeed());
		}

		public KeysPair GeneratePair(byte[] privateKey)
		{
			var publicKey = GetPublicKeyFromPrivateKeyEx(privateKey);
			return new KeysPair(privateKey, publicKey);
		}

		public KeysPair GeneratePair()
		{
			ECKeyGenerationParameters keyGenParams = new ECKeyGenerationParameters(domain, new SecureRandom());

			ECKeyPairGenerator generator = new ECKeyPairGenerator();
			generator.Init(keyGenParams);
			var pair = generator.GenerateKeyPair();

			ECPrivateKeyParameters privateKeyParameters = (ECPrivateKeyParameters) pair.Private;

			var privateKey = privateKeyParameters.D.ToByteArrayUnsigned();
			var publicKey = GetPublicKeyFromPrivateKeyEx(privateKey);

			return new KeysPair(privateKey, publicKey);
		}

		public byte[] GetPublicKeyFromPrivateKeyEx(byte[] privateKey)
		{
			var d = new BigInteger(privateKey);
			var q = domain.G.Multiply(d);

			var publicKey = new ECPublicKeyParameters(q, domain);

			return publicKey.Q.GetEncoded();
		}

		public byte[] GetSignature(byte[] privateKeyInfoData, byte[] data)
		{
			var keyParameters = new ECPrivateKeyParameters(new BigInteger(privateKeyInfoData), domain);
			var signer = SignerUtilities.GetSigner("SHA-256withECDSA");

			signer.Init(true, keyParameters);
			signer.BlockUpdate(data, 0, data.Length);

			var signature = signer.GenerateSignature();
			return signature;

		}

		public bool VerifySignature(byte[] data, byte[] publicKey, byte[] signature)
		{
			try
			{
				var q = ecParams.Curve.DecodePoint(publicKey);

				var keyParameters = new ECPublicKeyParameters(q, domain);
				var signer = SignerUtilities.GetSigner("SHA-256withECDSA");

				signer.Init(false, keyParameters);
				signer.BlockUpdate(data, 0, data.Length);

				return signer.VerifySignature(signature);
			}
			catch
			{
				return false;
			}
		}
	}
}