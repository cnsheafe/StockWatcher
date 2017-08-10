using Npgsql;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Data {
    public class StockRequestDb {
        private NpgsqlConnection conn = 
            new NpgsqlConnection(@"
                Host=localhost;
                Username=myUsername;
                Password=myPassword;
                Database=StockWatcher");
        private const string TABLENAME = "STOCK_REQUESTS";
        public void Init() {
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $@"
            CREATE TABLE IF NOT EXISTS {TABLENAME}(
                id serial PRIMARY KEY,
                username VARCHAR [],
                equity VARCHAR,
                price MONEY,
                request_uuid VARCHAR
                )";
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public bool CheckUuid(string uuid) {
            Init();
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $@"
                SELECT uuid from {TABLENAME}
                WHERE uuid='{uuid}'
                ";
            if (cmd.ExecuteNonQuery() > 0) {
                conn.Close();
                return false;
            }
            conn.Close();
            return true;
        }

        public void AddWatch(Stock stock) {
            Init();
            conn.Open();
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                INSERT INTO {TABLENAME}(
                    username, 
                    equity, 
                    price
                )
                VALUES(
                    {stock.Username},
                    {stock.Equity},
                    {stock.Price}
                )";
                cmd.ExecuteNonQuery();
            };
            conn.Close();
        }

        public string GetUuid(string username) {
            conn.Open();
            string uuid = "";
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                SELECT uuid FROM accounts 
                WHERE username = '{username}'";
                using (var reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        uuid = reader.GetString(0);
                    }
                }
            }
            conn.Close();
            return uuid;
        }
    }
}