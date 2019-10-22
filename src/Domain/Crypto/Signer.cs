namespace Domain.Crypto
{
	public class Signer
	{
		private readonly ICryptoService generator;

		public Signer(ICryptoService generator)
		{
			this.generator = generator;
		}

		public void Sign(BlockItem item, KeysPair keys)
		{
			var data = item.GetData();

			var signature = Sign(data, keys);

			item.Signature = signature;
			item.PublicKey = keys.PublicKey;
		}

		public byte[] Sign(byte[] data, KeysPair keys)
		{
			return Sign(data, keys.PrivateKey);
		}

		public byte[] Sign(byte[] data, byte[] privateKey)
		{
			return generator.GetSignature(privateKey, data);
		}
	}
}