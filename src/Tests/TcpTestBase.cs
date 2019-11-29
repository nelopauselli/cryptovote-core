using System;
using System.Threading;
using NUnit.Framework;

namespace Tests
{
	public class TcpTestBase
	{
		protected const int TcpDefaultTimeout = 10000;

		protected const string host1 = "127.0.0.1";
		protected const int port1 = 14001;
		protected const string host2 = "127.0.0.1";
		protected const int port2 = 14002;
		protected const string host3 = "127.0.0.1";
		protected const int port3 = 14003;

		protected void WaitFor(Func<bool> condition, int timeout = TcpDefaultTimeout)
		{
			var count = timeout / 250;
			while (count > 0)
			{
				if (condition.Invoke()) break;

				Thread.Sleep(250);
				count--;
			}
		}
	}
}