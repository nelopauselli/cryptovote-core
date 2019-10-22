using System;
using System.Diagnostics;

namespace Client
{
	public class AppConfig
	{
		public Uri WebSocketUrl { get; }
		public Uri RestUrl { get; }

		public AppConfig(string[] args)
		{
			var server = "cryptovote.azurewebsites.net";
			var schema = "https";

			if (args != null)
			{
				foreach (var arg in args)
				{
					Debug.WriteLine($"arg: {arg}");

					var chunks = arg.Split('=');
					if (chunks.Length != 2) continue;

					if (chunks[0].ToLower() == "server")
					{
						server = chunks[1];
					}

					if (chunks[0].ToLower() == "schema")
					{
						schema = chunks[1];
					}
				}
			}

			WebSocketUrl = new Uri($"ws://{server}/ws");
			RestUrl = new Uri($"{schema}://{server}/");
		}
	}
}