using Npgsql;

namespace StockWatcher.Model.Services.Helpers
{
    public static class ConnectionStringParser
    {
        /// <summary>
        /// Parses connection string of the form "postgres://username:password@host:portNumber/database"
        /// into connection string of form "Username=username;Password=password;Host=host;Port=portNumber;Database=database;"
        /// </summary>
        /// <param name="rawConnectionString">
        /// Connection string of the form "postgres://username:password@host:portNumber/database"
        /// </param>
        /// <returns>
        /// Connection string of the form "Username=username;Password=password;Host=host;Port=portNumber;Database=database;"
        /// </returns>
        public static string Parse(string rawConnectionString)
        {
            int firstIndex = rawConnectionString.IndexOf("//");
            int lastIndex = rawConnectionString.IndexOf("@");

            string usernamePassword = rawConnectionString.Substring(firstIndex + 2, lastIndex - firstIndex - 2);
            string[] tmp = usernamePassword.Split(":");
            string username = tmp[0];
            string password = tmp[1];

            firstIndex = lastIndex + 1;
            lastIndex = rawConnectionString.LastIndexOf("/");

            string hostPort = rawConnectionString.Substring(firstIndex, lastIndex - firstIndex);

            tmp = hostPort.Split(":");
            string host = tmp[0];
            int port = int.Parse(tmp[1]);

            string database = rawConnectionString.Substring(lastIndex + 1);
            
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
            connectionStringBuilder.Username = username;
            connectionStringBuilder.Password = password;
            connectionStringBuilder.Host = host;
            connectionStringBuilder.Port = port;
            connectionStringBuilder.Database = database;
            connectionStringBuilder.SslMode = SslMode.Prefer;
            connectionStringBuilder.TrustServerCertificate = true;

            return connectionStringBuilder.ConnectionString;
        }
    }
}