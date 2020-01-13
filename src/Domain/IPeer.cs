using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Domain
{
	public interface IPeer
	{
		string Id { get; }
		string Host { get; }
		int Port { get; }
		Task SendAsync(byte[] data, CancellationToken ctsToken);
		bool IsReady { get; }
		void GetBlock(byte[] hash);
	}
}