namespace Domain.Crypto
{
	public class SignatureVerify
	{
		private readonly ICryptoService service;

		public SignatureVerify()
			: this(new CryptoSecp256k1())
		{
		}

		public SignatureVerify(ICryptoService service)
		{
			this.service = service;
		}

		public bool Verify(BlockItem record)
		{
			var data = record.GetData();
			var publicKey = record.PublicKey;
			var signature = record.Signature;

			return Verify(data, publicKey, signature);
		}

		public bool Verify(byte[] data, byte[] publicKey, byte[] signature)
		{
			return service.VerifySignature(data, publicKey, signature);
		}
	}
}