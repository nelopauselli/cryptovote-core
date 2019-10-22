using System;

namespace Domain.Elections
{
	public class QuestionResult
	{
		public Guid QuestionId { get; set; }
		public ChoiceResult[] Choices { get; set; } = Array.Empty<ChoiceResult>();
		public byte Type { get; set; }
	}
}