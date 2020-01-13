using System;
using System.Text.Json;
using System.Threading;
using CryptoVote.Loggers;
using Domain;
using Domain.Converters;
using Domain.Elections;
using Microsoft.Extensions.Configuration;

namespace CryptoVote
{
	class Program
	{
		private static bool stop;

		static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.AddCommandLine(args)
				.AddEnvironmentVariables();

			var configuration = new NodeConfiguration(builder.Build());
			
			var logger = configuration.ConsoleColored ? (INodeLogger)new ColoredConsoleLogger(configuration.Verbosity) : new ConsoleLogger(configuration.Verbosity);
			logger.Information("Hello Crypto Node!");
			logger.Information($"name: {configuration.Name}");
			logger.Information($"miner:address: {configuration.MinerAddress}");
			logger.Information($"miner:interval: {configuration.MinerInterval} ms");
			logger.Information($"blockchain:dificulty: {configuration.BlockchainDificulty}");
			logger.Information($"my:host: {configuration.MyHost}");
			logger.Information($"peer:url: {configuration.PeerUrl}");
			logger.Information($"console:colored: {configuration.ConsoleColored}");
			logger.Information($"verbosity: {configuration.Verbosity}");
			
			var node = new Node(configuration, new BlockBuilder(), logger);

			node.Start();

			if (!string.IsNullOrWhiteSpace(configuration.PeerUrl))
			{
				logger.Information($"Intentando conectar con: {configuration.PeerUrl}");
				node.Connect(configuration.PeerUrl);
				node.Syncronize();

				node.Discovery();
			}

			Console.CancelKeyPress += OnCancelKeyPress;
			while (!stop)
				Thread.Sleep(100);

			Console.WriteLine("Stoping...");
			node.Stop();
			Console.WriteLine("Stoped");
		}

		private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Console.WriteLine("Invoke stoping");
			stop = true;
		}
	}
}