using System;

namespace Domain
{
	public class Scheduler
	{
		private readonly long minerInterval;
		private long minerLastExecutedTime;

		private readonly long peersCheckInterval;
		private long peersCheckLastExecutedTime;

		private readonly long syncronizeInterval;
		private long syncronizeLastExecutedTime;

		public Scheduler(in int minerInterval, in int peersCheckInterval, long syncronizeInterval)
		{
			this.minerInterval = minerInterval;
			this.minerLastExecutedTime = 0;
			this.peersCheckInterval = peersCheckInterval;
			this.peersCheckLastExecutedTime = 0;

			this.syncronizeInterval = syncronizeInterval;
			this.syncronizeLastExecutedTime = 0;
		}

		public bool IsTimeToCheckPeers()
		{
			var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			if (peersCheckLastExecutedTime + peersCheckInterval > now) return false;

			peersCheckLastExecutedTime = now;
			return true;
		}

		public bool IsTimeToSyncronize()
		{
			var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			if (syncronizeLastExecutedTime + syncronizeInterval > now) return false;

			syncronizeLastExecutedTime = now;
			return true;
		}

		public bool IsTimeToMine()
		{
			var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			if (minerLastExecutedTime + minerInterval > now) return false;

			minerLastExecutedTime = now;
			return true;
		}
	}
}