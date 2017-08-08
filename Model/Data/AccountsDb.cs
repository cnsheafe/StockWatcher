using System;
using Npgsql;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Data {
    // Manage Accounts
    public class AccountsDb {
        private NpgsqlConnection conn = new NpgsqlConnection(@"
        Host=localhost;
        Username=myUsername;
        Password=myPassword;
        Database=StockWatcher");

        private const string TABLENAME = "ACCOUNTS";

        public void Init() {
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $@"
            CREATE TABLE IF NOT EXISTS {TABLENAME}(
                id serial PRIMARY KEY,
                username VARCHAR UNIQUE NOT NULL,
                password VARCHAR NOT NULL,
                email VARCHAR,
                uuid VARCHAR UNIQUE NOT NULL)";
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        // TODO: Have to add uuid checker to prevent collison
        public bool Add(User user) {
            user.uuid = Guid.NewGuid().ToString();
            Init();
            conn.Open();
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                INSERT INTO {TABLENAME} (username, password, uuid)
                VALUES (
                    '{user.username}',
                    '{user.password}',
                    '{user.uuid}'
                )
                ON CONFLICT (username)
                DO NOTHING";

                if (cmd.ExecuteNonQuery() == 0) {
                    Console.WriteLine("Row already exists");
                    conn.Close();
                    return false;
                }
                conn.Close();
                return true;
            }
        }
    }
}