using System;

namespace tinyProfiler
{
    public class SqlCapturedEventArgs: EventArgs
    {
        private SqlProfilerRow sqlProfilerRow;

        public string Text { get { return sqlProfilerRow.Text; } }
        public long RowCount { get { return sqlProfilerRow.RowCount; } }
        public DateTime Time { get { return sqlProfilerRow.Time; } }

        public SqlCapturedEventArgs(SqlProfilerRow row) 
        {
            this.sqlProfilerRow = row;
        }
    }
}
