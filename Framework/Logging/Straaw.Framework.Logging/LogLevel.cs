namespace Straaw.Framework.Logging
{
	/// <summary>
	/// This class is used to represent log severities. You cannot create your own instances. The only
	/// available instances are created automatically at startup and made available as static members.
	/// </summary>
	public class LogLevel
	{
		public static LogLevel Verbose = new LogLevel(0, "V", "VRB", "Verbose");
		public static LogLevel Debug = new LogLevel(1, "D", "DBG", "Debug");
		public static LogLevel Info = new LogLevel(2, "I", "INF", "Information");
		public static LogLevel Warning = new LogLevel(3, "W", "WRN", "Warning");
		public static LogLevel Error = new LogLevel(4, "E", "ERR", "Error");
		public static LogLevel FatalError = new LogLevel(5, "F", "FAT", "Fatal Error");
		public static LogLevel None = new LogLevel(6, "N", "N/A", "No Logging");

		public int    Ordinal { get; private set; }
		public string Code { get; private set; }
		public string ShortName { get; private set; }
		public string LongName { get; private set; }

		private LogLevel(int ordinal, string code, string shortName, string longName)
		{
			Ordinal = ordinal;
			Code = code;
			ShortName = shortName;
			LongName = longName;
		}
	}
}
