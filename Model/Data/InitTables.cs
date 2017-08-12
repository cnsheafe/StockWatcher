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
                const string REQ_TABLE = "stock_requests";
                const string ACCT_TABLE = "accounts";
                const string REQUEST_ID = "request_id";

                using (var reqCmd = new NpgsqlCommand()) {
                    reqCmd.Connection = conn;
                    reqCmd.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS {REQ_TABLE}(
                        id serial PRIMARY KEY,
                        usernames VARCHAR [],
                        equity VARCHAR,
                        price MONEY,
                        {REQUEST_ID} VARCHAR
                    )";
                    reqCmd.ExecuteNonQuery();
                }

                using (var acctCmd = new NpgsqlCommand()) {
                    acctCmd.Connection = conn;
                    acctCmd.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS {ACCT_TABLE}(
                        id serial PRIMARY KEY,
                        username VARCHAR UNIQUE NOT NULL,
                        password VARCHAR NOT NULL,
                        phone VARCHAR UNIQUE NOT NULL,
                        uuid VARCHAR UNIQUE NOT NULL
                    )";
                    acctCmd.ExecuteNonQuery();
                }
                conn.Close();
            }

        }
    }
}
