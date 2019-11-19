namespace Domain.Channels.Protocol
{
	public class CommandIds
	{
		public const byte NotFound = 0;

		public const byte Login = 1;
		public const byte Authorized = 2;
		public const byte Ping = 3;
		public const byte Pong = 4;
		public const byte Echo = 5;
		public const byte EchoReply = 6;
		
		public const byte Document = 7;
		public const byte DocumentHash = 8;

		
		public const byte SendBlock = 40;
		public const byte SendDocument = 41;
		
		public const byte SendVote = 50;
		public const byte SendUrn = 51;
		public const byte SendMember = 52;
		public const byte SendQuestion = 53;
		public const byte SendCommunity = 54;
		public const byte SendFiscal = 55;
		public const byte SendRecount = 56;
		public const byte GetLastBlock = 57;
		public const byte GetBlock = 58;


		public const byte QueryRequestPeers = 100;
		public const byte QueryResponsePeers = 101;

		public const byte QueryLastBlock = 102;
		public const byte QueryBlock = 103;
		
		public const byte QueryResponse = 110;
		public const byte QueryCommunities = 111;
		public const byte QueryCommunity = 112;

		public const byte QueryQuestions= 113;
		public const byte QueryQuestion = 114;

		public const byte QueryMembers = 115;
		public const byte QueryMember = 116;

	}
}