using System;
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
                Database=StockWatcher"
            );
        
        // Constants for TABLE NAMES and fields
        // TODO: Change REQUESTS to REQUEST_HISTORY
        private const string REQUESTS = "stock_requests";
        private const string USER_REQUESTS = "user_requests";
        private const string REQUEST_ID = "request_id";
        public bool IsRunning(string requestId) {
            conn.Open();
            var cmd = new NpgsqlCommand();
            bool status = false;

            cmd.Connection = conn;
            cmd.CommandText = $@"
                SELECT {REQUEST_ID} from {USER_REQUESTS}
                WHERE {REQUEST_ID} ='{requestId}'
            ";
            using (var reader = cmd.ExecuteReader()) {
                if (reader.HasRows) {
                    status = true;
                    Console.WriteLine("Request is already running");
                }
                reader.Close();

            }
            conn.Close();
            return status;
        }

        public void Add(Stock stock) {
            bool IsDuplicate = false;
            conn.Open();

            // Query for duplicate request in user_requests 
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                SELECT {REQUEST_ID} FROM {USER_REQUESTS}
                WHERE {REQUEST_ID} = '{stock.RequestId}'
                AND username = '{stock.Username}'
                ";
                using (var reader = cmd.ExecuteReader()) {
                    if (reader.HasRows) {
                        IsDuplicate = true;
                        Console.WriteLine("Request already exists");
                    }
                    reader.Close();
                }
            }

            if (!IsDuplicate) {
                Console.WriteLine("Is not a duplicate");
                using (var cmd = new NpgsqlCommand()) {
                    cmd.Connection = conn;
                    cmd.CommandText = $@"
                    INSERT INTO {USER_REQUESTS}(
                        request_id,
                        username
                    )
                    VALUES(
                        '{stock.RequestId}',
                        '{stock.Username}'
                    )
                    ";
                    cmd.ExecuteNonQuery();
                }
            }
            conn.Close();
        }

        public void Remove(string requestId) {
            conn.Open();
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                DELETE FROM {USER_REQUESTS}
                WHERE {REQUEST_ID} = '{requestId}'
                ";
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }

        public List<string> GetUsers(string requestId) {
            conn.Open();
            var users = new List<string>();

            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                SELECT username FROM {USER_REQUESTS}
                WHERE {REQUEST_ID} = '{requestId}'
                ";
                using (var reader = cmd.ExecuteReader()) {
                    if (reader.HasRows) {
                        while(reader.Read()) {
                            users.Add(reader.GetString(0));
                            Console.WriteLine(reader.GetString(0));
                        };
                    }
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
                UPDATE {REQUESTS}
                SET users = array_remove(users, 'username')
                ";
                cmd.ExecuteNonQuery();
            }
        }
    }
}