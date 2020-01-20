using System;

namespace Domain
{
	public class Peer
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string PublicUrl { get; set; }
		public DateTimeOffset LastActivity { get; set; }

		public override string ToString()
		{
			return $"{Name} ({PublicUrl})";
		}
	}
}