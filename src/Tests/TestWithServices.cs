using Domain;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests
{
	public class TestWithServices
	{
		protected ILogger<Node> loggerNode;

		protected const string host1 = "localhost";
		protected const int port1 = 8001;
		protected const string host2 = "localhost";
		protected const int port2 = 8002;
		protected const string host3 = "localhost";
		protected const int port3 = 8003;

		[OneTimeSetUp]
		public void InitLogger()
		{
			using (var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole()))
			{
				loggerNode = loggerFactory.CreateLogger<Node>();
			}
		}
	}
}