using System;
using System.Linq;
using Domain;

namespace Client
{
	internal class Screen
	{
		private const int column2 = 30;

		private int lastWidth;
		private int lastHeight;
		private readonly ILogger logger;
		private readonly AppConfig config;
		private readonly Blockchain blockchain;
		private readonly IStorage storage;
		private readonly WebSocketChannel channel;
		private readonly Counter counter;

		public Screen(ILogger logger, AppConfig config, Blockchain blockchain, IStorage storage, WebSocketChannel channel, Counter counter)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.config = config ?? throw new ArgumentNullException(nameof(config));
			this.blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
			this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
			this.channel = channel ?? throw new ArgumentNullException(nameof(channel));
			this.counter = counter ?? throw new ArgumentNullException(nameof(counter));
		}

		public void Draw(long nextTimeToMine)
		{
			var width = Console.WindowWidth;
			var height = Console.WindowHeight;
			var row = 0;

			if (lastHeight != height || lastWidth != width)
			{
				Console.Clear();
				lastHeight = height;
				lastWidth = width;
			}

			Console.CursorVisible = false;
			Console.ForegroundColor = ConsoleColor.DarkGreen;

			//Console.SetCursorPosition(0, row++);
			//Console.Write("> ");

			Console.SetCursorPosition(0, row++);
			Console.Write(new string('=', width-15));
			Console.Write("[");
			Console.Write(DateTimeOffset.UtcNow.ToString("HH:mm:ss"));
			Console.Write("]");
			Console.Write(new string('=', 5));

			Console.SetCursorPosition(1, row);
			if (storage.LastVote != null)
			{
				var lastVoteTime = DateTimeOffset.FromUnixTimeMilliseconds(storage.LastVote.Time);
				Console.Write($"Last vote at {lastVoteTime:HH:mm:ss}".PadRight(column2));
			}

			Console.SetCursorPosition(column2, row++);
			Console.Write($"Channel State {channel.State} ".PadLeft(width-column2));

			Console.SetCursorPosition(1, row);
			Console.Write($"Incoming transactions {storage.Incomming}".PadRight(column2));
			Console.SetCursorPosition(column2, row++);
			Console.Write($"WebSocket Url {config.WebSocketUrl} ".PadLeft(width - column2));

			Console.SetCursorPosition(1, row);
			Console.Write($"Pending transactions {storage.Transactions.Count}".PadRight(column2));
			Console.SetCursorPosition(column2, row++);
			Console.Write($"Rest Api Url {config.RestUrl} ".PadLeft(width - column2));

			row++;
			Console.SetCursorPosition(1, row++);
			var timespan = DateTimeOffset.FromUnixTimeSeconds(nextTimeToMine).Subtract(DateTimeOffset.Now).TotalSeconds;
			Console.Write($"Blockchain size: {blockchain.Chain.Count()} - {(blockchain.IsValid() ? "It's valid :D" : "It isn't valid :(")} - Next mining in {timespan:F2} seconds".PadRight(width));

			row++;
			Console.SetCursorPosition(1, row++);
			Console.Write("Result:");

			Console.SetCursorPosition(3, row++);
			Console.Write($"Vote for choice #1 {counter.TotalFor(1)}");
			Console.SetCursorPosition(3, row++);
			Console.Write($"Vote for choice #2 {counter.TotalFor(2)}");
			Console.SetCursorPosition(3, row++);
			Console.Write($"Vote for choice #3 {counter.TotalFor(3)}");
			Console.SetCursorPosition(3, row++);
			Console.Write($"Vote for choice #4 {counter.TotalFor(4)}");

			row = Math.Max(row, height - 11);
			Console.SetCursorPosition(0, row++);
			Console.Write(new string('=', 5));
			Console.Write("[ log ]");
			Console.Write(new string('=', width - 12));

			Console.SetCursorPosition(0, row++);

			Console.ResetColor();
			foreach (var log in logger.Logs.TakeLast(10))
			{
				Console.ForegroundColor = log.Type.Color;
				Console.Write($"[{log.Time:HH:mm:ss}] {log.Message}".PadRight(width).Substring(0, width));
			}

			//Console.CursorVisible = true;
			Console.SetCursorPosition(2, 0);
		}

		
	}
}