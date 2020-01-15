using Domain;
using Domain.Elections;
using Microsoft.Extensions.DependencyInjection;

namespace crypto_vote
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBlockchain(this IServiceCollection services)
		{
			services.AddSingleton<INodeConfiguration, NodeConfiguration>();
			services.AddSingleton<IBlockBuilder, BlockBuilder>();
			services.AddSingleton<IPeerChannel, HttpPeerChannel>();
			services.AddSingleton<INode, Node>();

			return services;
		}
	}
}