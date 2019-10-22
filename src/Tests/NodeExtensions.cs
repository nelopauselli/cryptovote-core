using System;
using System.Linq;
using System.Threading;
using Domain;

namespace Tests
{
	public static class NodeExtensions
	{
		public static void WaitPendingTransactions(this Node node, int expected = 1, int timeout = 2000)
		{
			var timeLimit = DateTime.Now.AddMilliseconds(timeout);
			do
			{
				var count = node.Pendings.Count();
				if (count >= expected)
					break;
				if (DateTime.Now > timeLimit)
					break;
				Thread.Sleep(10);
			} while (true);
		}

	}
}