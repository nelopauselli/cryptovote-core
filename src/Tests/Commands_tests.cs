using System.IO;
using System.Text;
using Domain.Channels.Protocol;
using Domain.Queries;
using NUnit.Framework;

namespace Tests
{
	public class Commands_tests
	{
		[Test]
		public void Communities_list()
		{
			using (var memory = new MemoryStream())
			{
				var command = new CommunitiesQueryCommand();
				command.Send(memory);

				memory.Seek(0, SeekOrigin.Begin);

				var header = CommandHeader.Parse(memory);
				Assert.IsNotNull(header);
				Assert.AreEqual(CommandIds.QueryCommunities, header.CommandId);
				Assert.AreEqual(0, header.Length);
			}
		}
	}
}