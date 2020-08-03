using System;

namespace tinyProfiler
{
    class Program
    {
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

            if (string.IsNullOrEmpty(_instance) 
                || string.IsNullOrEmpty(_database) 
                || string.IsNullOrEmpty(_username) 
                || string.IsNullOrEmpty(_password)) 
            {
                Console.WriteLine("Error in parsing arguments.\n\nUsage: tinyProfiler.exe -i=YOUR_SERVER\\SQL_INSTANCE -d=DATABASE -u=USERNAME -p=PASSWORD");
                Environment.Exit(-1);
            }

            Console.WriteLine($"Start tracing on database {_database}. Press 'q' to stop tracing...");

            _profiler = new SqlProfiler(_instance, _database, _username, _password);
            _profiler.SqlCaptured += _profiler_SqlCaptured;
            _profiler.Run();

            if (Console.ReadKey(true).KeyChar == 'q')
            {
                _profiler.Stop();
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
            Console.WriteLine("----- Event ID #" + (++_numEvents).ToString() + "\n" + e.Text.Trim() + "\n----- Row count: " + e.RowCount.ToString() + "\n" + "----- Timestamp: " + e.Time.ToUniversalTime().ToString());
        }
    }
}
