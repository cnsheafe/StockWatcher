using Npgsql;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Data {
    public class StockSurveyDb {
        private NpgsqlConnection conn = 
            new NpgsqlConnection(@"
                Host=localhost;
                Username=myUsername;
                Password=myPassword;
                Database=StockWatcher");
        private const string TABLENAME = "STOCK_SURVEYS";
        public void Init() {
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $@"
            CREATE TABLE IF NOT EXISTS {TABLENAME}(
                id serial PRIMARY KEY,
                uuid VARCHAR NOT NULL,
                equity VARCHAR,
                price MONEY)";
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
            var cmd = new NpgsqlCommand();
            cmd.CommandText = $@"
            INSERT INTO {TABLENAME}(
                uuid, 
                equity, 
                price
            )
            VALUES(
                {stock.uuid},
                {stock.equity},
                {stock.price}
            )";
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}