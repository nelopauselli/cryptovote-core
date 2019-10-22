using System;

namespace Client
{
	public abstract class LogType
	{
		public static LogType WARN=>new LogTypeWarning();
		public static LogType DEBUG=>new LogTypeDebug();
		public static LogType INFO=>new LogTypeInfo();
		public static LogType ERROR => new LogTypeError();

		public abstract ConsoleColor Color { get; }
	}

	class LogTypeDebug : LogType
	{
		public override ConsoleColor Color => ConsoleColor.DarkGray;
	}
	class LogTypeInfo : LogType
	{
		public override ConsoleColor Color => ConsoleColor.Green;
	}

	class LogTypeError : LogType
	{
		public override ConsoleColor Color => ConsoleColor.DarkRed;
	}

	class LogTypeWarning : LogType
	{
		public override ConsoleColor Color => ConsoleColor.Yellow;
	}

}