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
			services.AddTransient<IBlockBuilder, BlockBuilder>();
			services.AddTransient<IPeerChannel, HttpPeerChannel>();
			services.AddSingleton<INode, Node>();

			return services;
		}
	}
}