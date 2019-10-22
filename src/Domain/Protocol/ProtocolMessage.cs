namespace Domain.Protocol
{
	public abstract class ProtocolMessage
	{
		public const char SendQuestionCommandId = 'Q';
		public const char SendCommunityCommandId = 'C';
		public const char SendBlockCommandId = 'B';
		public const char SendDocumentCommandId = 'D';
		public const char SendFiscalCommandId = 'F';
		public const char SendMemberCommandId = 'M';
		public const char SendPeerCommandId = 'P';
		public const char SendRecountCommandId = 'R';
		public const char SendUrnCommandId = 'U';
		public const char SendVoteCommandId = 'V';
		public const char BlockQueryCommandId = 'X';
		public const char LastBlockQueryCommandId = 'L';
		public const char QueryCommandId = '?';

		public abstract byte[] GetBytes();
	}
}