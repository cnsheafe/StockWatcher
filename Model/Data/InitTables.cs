using Npgsql;

namespace StockWatcher.Model.Data {
    public static class InitTables {
        public static void Build() {
            using (var conn = new NpgsqlConnection()) {
                conn.ConnectionString =@"
                Host=localhost;
                Username=myUsername;
                Password=myPassword;
                Database=StockWatcher";

                conn.Open();
                const string REQUESTS = "stock_requests";
                const string ACCOUNTS = "accounts";
                const string USER_REQUESTS = "user_requests";
                const string REQUEST_ID = "request_id";

                using (var reqCmd = new NpgsqlCommand()) {
                    reqCmd.Connection = conn;
                    reqCmd.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS {REQUESTS}(
                        id serial PRIMARY KEY,
                        username TEXT,
                        equity TEXT,
                        price MONEY,
                        {REQUEST_ID} TEXT,
                        request_time TIMESTAMPZ
                    )";
                    reqCmd.ExecuteNonQuery();
                }

                using (var acctCmd = new NpgsqlCommand()) {
                    acctCmd.Connection = conn;
                    acctCmd.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS {ACCOUNTS}(
                        id serial PRIMARY KEY,
                        username TEXT UNIQUE NOT NULL,
                        password TEXT NOT NULL,
                        phone TEXT UNIQUE NOT NULL,
                        uuid TEXT UNIQUE NOT NULL
                    )";
                    acctCmd.ExecuteNonQuery();
                }

                using (var jointCmd = new NpgsqlCommand()) {
                    jointCmd.Connection = conn;
                    jointCmd.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS {USER_REQUESTS}(
                        row_id serial PRIMARY KEY,
                        {REQUEST_ID} VARCHAR(256) NOT NULL,
                        username VARCHAR(256) NOT NULL
                    )";
                    jointCmd.ExecuteNonQuery();
                }
                conn.Close();
            }

        }
    }
}
