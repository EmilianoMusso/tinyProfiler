using System;
using System.Diagnostics;

namespace tinyProfiler
{
    class Program
    {
        static Stopwatch stopwatch = new Stopwatch();
        static long _numEvents = 0;
        static SqlProfiler _profiler = null;

        static void Main(string[] args)
        {
            var parser = new ArgsParser();
            parser.Parse(args);

            var _instance = parser.Argument("i");
            var _database = parser.Argument("d");
            var _username = parser.Argument("u");
            var _password = parser.Argument("p");

            ColorPrompt("£tinyProfiler - T-SQL profiler for SQL SERVER\n" + new string('-', 50), ConsoleColor.Green);

            if (string.IsNullOrEmpty(_instance) 
                || string.IsNullOrEmpty(_database) 
                || string.IsNullOrEmpty(_username) 
                || string.IsNullOrEmpty(_password)) 
            {
                Console.WriteLine("Error in parsing arguments.\n\nUsage: tinyProfiler.exe -i=YOUR_SERVER\\SQL_INSTANCE -d=DATABASE -u=USERNAME -p=PASSWORD");
                Environment.Exit(-1);
            }

            ColorPrompt($"Start tracing on database {_database}. Press £'q' to stop tracing...", ConsoleColor.Red);

            _profiler = new SqlProfiler(_instance, _database, _username, _password);
            _profiler.SqlCaptured += _profiler_SqlCaptured;
            _profiler.Run();

            stopwatch.Start();

            if (Console.ReadKey(true).KeyChar == 'q')
            {
                _profiler.Stop();

                stopwatch.Stop();
                ColorPrompt("\ntinyProfiler stopped. Tracing time: £" + stopwatch.Elapsed.ToString(), ConsoleColor.Red);

                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Output results through SqlProfiler class SqlCaptured event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void _profiler_SqlCaptured(object sender, SqlCapturedEventArgs e)
        {
            ColorPrompt("\n----- Event ID #£" + (++_numEvents).ToString(), ConsoleColor.Red);
            ColorPrompt(e.Text.Trim(), ConsoleColor.DarkYellow);
            ColorPrompt("----- Row count: £" + e.RowCount.ToString(), ConsoleColor.Red);
            ColorPrompt("----- Timestamp: £" + e.Time.ToString("yyyyMMddTHHmmss"), ConsoleColor.Red);
        }

        /// <summary>
        /// Search for £ character, used for coloring the word it precedes
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        private static void ColorPrompt(string message, ConsoleColor color)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            var pos = message.IndexOf("£");
            var psp = message.IndexOf(" ", pos + 1);
            if (psp == -1) psp = message.Length -1;

            if (pos == -1)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message.Substring(0, pos).Replace("£", ""));
                Console.ForegroundColor = color;
                Console.Write(message.Substring(pos + 1, psp - pos).Replace("£", ""));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message.Substring(psp + 1).Replace("£", ""));
            }
        }
    }
}
