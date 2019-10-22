using System;
using System.Threading;
using CryptoVote.Loggers;
using Domain;
using Domain.Channels;
using Domain.Converters;
using Domain.Scrutiny;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CryptoVote
{
	class Program
	{
		private static bool stop = false;

		static void Main(string[] args)
		{
			Console.WriteLine("Hello Crypto Node!");

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
			
			var logger = configuration.ConsoleColored ?  (IEventListener)new ColoredConsoleLogger() : new ConsoleLogger();
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