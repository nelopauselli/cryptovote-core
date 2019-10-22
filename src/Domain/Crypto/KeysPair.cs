namespace Domain.Crypto
{
	public class KeysPair
	{
		public KeysPair(byte[] privateKey, byte[] publicKey)
		{
			PrivateKey = privateKey;
			PublicKey = publicKey;
		}

		public byte[] PrivateKey { get; }
		public byte[] PublicKey { get; }
	}
}