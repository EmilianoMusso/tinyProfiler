using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace tinyProfiler
{
    public class SqlProfiler: IDisposable
    {
        public event EventHandler<SqlCapturedEventArgs> SqlCaptured;

        protected void OnSqlCaptured(SqlCapturedEventArgs e)
        {
            var handler = SqlCaptured;
            if (handler != null) handler(this, e);
        }

        // Profiling query
        private const string _TRACER = @"SELECT SQT.text,
                                                REQ.row_count
                                        FROM sys.dm_exec_requests AS REQ LEFT  JOIN  sys.databases DBS ON DBS.database_id = REQ.database_id
                                                                         LEFT  JOIN  sys.syslogins USR ON USR.sid = REQ.user_id
                                                                         CROSS APPLY sys.dm_exec_sql_text(REQ.sql_handle) AS SQT
                                        WHERE DBS.name LIKE @DatabaseName AND
                                              SQT.text NOT LIKE '%sys.dm_exec_sql_text%'";

        // Sql Server connection object
        private SqlConnection _connection;

        public string Instance { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsRunning { get; set; } = false;

        /// <summary>
        /// Returns a SQL Server connection string
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            return $@"Password={this.Password};Persist Security Info=True;User ID={this.Username};Data Source={this.Instance}";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SqlProfiler(string _instance, string _database, string _username, string _password)
        {
            this.Instance = _instance;
            this.Database = _database;
            this.Username = _username;
            this.Password = _password;

            _connection = new SqlConnection(this.GetConnectionString());
            _connection.Open();
        }

        /// <summary>
        /// Starts the database tracing
        /// </summary>
        public void Run()
        {
            this.IsRunning = true;

            Task.Run(() =>
            {
                while (this.IsRunning)
                {
                    using (var sqlCmd = new SqlCommand(_TRACER, _connection))
                    {
                        sqlCmd.Parameters.Add(new SqlParameter("DatabaseName", this.Database));

                        using (var _dr = sqlCmd.ExecuteReader())
                        {
                            if (_dr.HasRows)
                            {
                                while (_dr.Read())
                                {
                                    OnSqlCaptured(new SqlCapturedEventArgs(
                                        new SqlProfilerRow()
                                        {
                                            Text = _dr.GetString("text"),
                                            RowCount = _dr.GetInt64("row_count"),
                                            Time = DateTime.Now
                                        }
                                    ));
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Stops the database tracing
        /// </summary>
        public void Stop()
        {
            this.IsRunning = false;
        }

        /// <summary>
        /// Dispose SqlProfiler
        /// </summary>
        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

    }
}
