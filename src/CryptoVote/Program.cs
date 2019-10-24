using System;
using System.Threading;
using CryptoVote.Loggers;
using Domain;
using Domain.Converters;
using Domain.Elections;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CryptoVote
{
	class Program
	{
		private static bool stop;

		static void Main(string[] args)
		{
			JsonConvert.DefaultSettings = () =>
			{
				var settings = new JsonSerializerSettings();
				settings.Converters.Add(new GuidJsonConverter());
				settings.Converters.Add(new DatetimeOffsetJsonConverter());
				settings.Converters.Add(new ByteArrayJsonConverter());
				return settings;
			};

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
			logger.Information($"my:port: {configuration.MyPort}");
			logger.Information($"peer:host: {configuration.PeerHost}");
			logger.Information($"peer:port: {configuration.PeerPort}");
			logger.Information($"console:colored: {configuration.ConsoleColored}");
			logger.Information($"verbosity: {configuration.Verbosity}");
			
			var node = new Node(configuration, new BlockBuilder(), logger);

			node.Listen();
			node.Start();

			if (!string.IsNullOrWhiteSpace(configuration.PeerHost))
			{
				logger.Information($"Intentando conectar con: {configuration.PeerHost}:{configuration.PeerPort}");
				node.Register(configuration.PeerHost, configuration.PeerPort);
				node.Syncronize();
			}

			Console.CancelKeyPress += OnCancelKeyPress;
			while (!stop)
				Thread.Sleep(100);

			Console.WriteLine("Stoping...");
			node.Stop(2000);
			Console.WriteLine("Stoped");
		}

		private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Console.WriteLine("Invoke stoping");
			stop = true;
		}
	}
}