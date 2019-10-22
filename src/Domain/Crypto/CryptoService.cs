namespace Domain.Crypto
{
	public class CryptoService
	{
		public static ICryptoService Instance { get; } = new CryptoSecp256k1();
	}
}