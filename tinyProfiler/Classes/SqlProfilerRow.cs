using System;

namespace tinyProfiler
{
    public class SqlProfilerRow
    {
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public long RowCount { get; set; }
    }
}
