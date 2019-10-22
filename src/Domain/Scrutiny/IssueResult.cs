using System;

namespace Domain.Scrutiny
{
	public class IssueResult
	{
		public Guid IssueId { get; set; }
		public ChoiceResult[] Choices { get; set; } = Array.Empty<ChoiceResult>();
		public byte Type { get; set; }
	}

	public class ChoiceResult
	{
		public Guid ChoiceId { get; set; }
		public string Text { get; set; }
		public uint Color { get; set; }
		public long Votes { get; set; }
	}
}