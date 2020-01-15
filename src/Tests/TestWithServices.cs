using Microsoft.Extensions.Logging;

namespace Tests
{
	public class TestWithServices
	{
		protected ILoggerFactory CreateLoggerFactory()
		{
			return LoggerFactory.Create(builder => builder.AddConsole());
		}

		protected const string host1 = "localhost";
		protected const int port1 = 8001;
		protected const string host2 = "localhost";
		protected const int port2 = 8002;
		protected const string host3 = "localhost";
		protected const int port3 = 8003;
	}
}