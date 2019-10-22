using Domain.Protocol;

namespace Domain.Queries
{
	public interface IMessage<T>
	{
		byte[] GetBytes();
		T Parse(ProtocolMessageChannel channel);
	}
}