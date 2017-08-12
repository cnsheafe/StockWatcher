using System.Collections.Generic;
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
        public bool IsRunning(string requestId) {
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
            bool IsDuplicate = false;
            conn.Open();

            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                SELECT {REQUEST_ID} FROM {TABLE}
                WHERE {REQUEST_ID} = '{stock.RequestId}'";
                if(cmd.ExecuteNonQuery() > 0) 
                    IsDuplicate = true;
            }

            if (IsDuplicate) {
                using (var cmd = new NpgsqlCommand()) {
                    cmd.Connection = conn;
                    cmd.CommandText = $@"
                    UPDATE {TABLE}
                    SET usernames = usernames || '{stock.Username}'
                    WHERE {REQUEST_ID} = '{stock.RequestId}'
                    ";
                    cmd.ExecuteNonQuery();
                }
            }

            else {
                using (var cmd = new NpgsqlCommand()) {
                    cmd.Connection = conn;
                    cmd.CommandText = $@"
                    INSERT INTO {TABLE}(
                        usernames, 
                        equity, 
                        price,
                        {REQUEST_ID}
                    )
                    VALUES(
                        ARRAY ['{stock.Username}'],
                        '{stock.Equity}',
                        '{stock.Price}',
                        '{stock.RequestId}'
                    )";
                    cmd.ExecuteNonQuery();
                };
            }
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
                cmd.ExecuteNonQuery();
            }
        }

        public List<string> GetUsers(string requestId) {
            conn.Open();
            var users = new List<string>();

            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                SELECT {REQUEST_ID} FROM {TABLE}
                WHERE {REQUEST_ID} = '{requestId}'
                ";
                using (var reader = cmd.ExecuteReader()) {
                    while(reader.Read())
                        users.Add(reader.GetString(0));
                }
            }
            conn.Close();
            return users;
        }

        public void RemoveUser(string username) {
            conn.Open();
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                UPDATE {TABLE}
                SET users = array_remove(users, 'username')
                ";
                cmd.ExecuteNonQuery();
            }
        }
    }
}