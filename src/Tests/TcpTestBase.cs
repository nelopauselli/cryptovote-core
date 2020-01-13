using System;
using System.Threading;
using NUnit.Framework;

namespace Tests
{
	public class TcpTestBase
	{
		protected const int TcpDefaultTimeout = 10000;

		protected const string host1 = "localhost";
		protected const int port1 = 8001;
		protected const string host2 = "localhost";
		protected const int port2 = 8002;
		protected const string host3 = "localhost";
		protected const int port3 = 8003;
	}
}