namespace Domain.Crypto
{
	public interface ICryptoService
	{
		KeysPair GeneratePair();
		byte[] GetSignature(byte[] privateKeyInfoData, byte[] data);
		bool VerifySignature(byte[] data, byte[] publicKey, byte[] signature);
	}
}