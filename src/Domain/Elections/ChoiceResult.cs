using System;

namespace Domain.Elections
{
	public class ChoiceResult
	{
		public Guid ChoiceId { get; set; }
		public string Text { get; set; }
		public uint Color { get; set; }
		public long Votes { get; set; }
	}
}