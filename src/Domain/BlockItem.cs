using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.Crypto;

namespace Domain
{
	public abstract class BlockItem
	{
		[JsonIgnore]
		public IList<string> Messages { get; } = new List<string>();

		public byte[] PublicKey { get; set; }
		public byte[] Signature { get; set; }

		public abstract string GetKey();
		public abstract byte[] GetData();

		public virtual bool IsValid(IList<Block> chain)
		{
			Messages.Clear();

			var signatureVerify = new SignatureVerify(CryptoService.Instance);
			if (PublicKey == null)
			{
				Messages.Add("El item no tiene clave pública");
				return false;
			}

			if (Signature == null)
			{
				Messages.Add("El item no tiene firma");
				return false;
			}

			if (!signatureVerify.Verify(this))
			{
				Messages.Add($"La firma no es válida. Raw: {BitConverter.ToString(GetData())}");
				return false;
			}

			return true;
		}
	}
}