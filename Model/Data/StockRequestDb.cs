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
        private const string TABLE= "stock_requests";
        private const string REQUEST_ID = "request_id";
        public void Init() {
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $@"
            CREATE TABLE IF NOT EXISTS {TABLE}(
                id serial PRIMARY KEY,
                usernames VARCHAR [],
                equity VARCHAR,
                price MONEY,
                {REQUEST_ID} VARCHAR UNIQUE
                )";
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public bool IsRunning(string requestId) {
            // Init();
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $@"
                SELECT {REQUEST_ID} from {TABLE}
                WHERE {REQUEST_ID} ='{requestId}'
                ";
            if (cmd.ExecuteNonQuery() > 0) {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        }

        public void Add(Stock stock) {
            // Init();
            conn.Open();
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                INSERT INTO {TABLE}(
                    usernames, 
                    equity, 
                    price
                )
                VALUES(
                    '{stock.Username}',
                    '{stock.Equity}',
                    '{stock.Price}'
                )";
                cmd.ExecuteNonQuery();
            };
            conn.Close();
        }

        public void Remove(string requestId) {
            conn.Open();
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                DELETE FROM {TABLE}
                WHERE {REQUEST_ID} = '{requestId}'
                ";
            }
        }
    }
}