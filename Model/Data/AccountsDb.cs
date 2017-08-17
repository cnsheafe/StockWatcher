using System;
using Npgsql;
using StockWatcher.Model.Schemas;

    // TODO: Have to add uuid checker to prevent collison
namespace StockWatcher.Model.Data {
    /// <summary>
    /// Manages user account information.
    /// </summary>
    public class AccountsDb {
        private NpgsqlConnection conn = new NpgsqlConnection(@"
        Host=localhost;
        Username=myUsername;
        Password=myPassword;
        Database=StockWatcher");

        private const string ACCOUNTS = "ACCOUNTS";
        /// <summary>
        /// Adds a user to the table
        /// </summary>
        /// <param name="user">User info including phone number</param>
        /// <para/>
        /// <returns>True if user was added to the table. Otherwise False.</returns>
        public void Add(User user) {
            user.Uuid = Guid.NewGuid().ToString();
            conn.Open();
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                INSERT INTO {ACCOUNTS} (username, password, phone, uuid)
                VALUES (
                    '{user.Username}',
                    '{user.Password}',
                    '{user.Phone}',
                    '{user.Uuid}'
                )
                ON CONFLICT (username)
                DO NOTHING";
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
        public string GetUuid(string username) {
            conn.Open();
            string uuid = "";
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                SELECT uuid FROM {ACCOUNTS}
                WHERE username = '{username}'
                ";
                using (var reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        Console.WriteLine("In the loop");
                        uuid = reader.GetString(0);
                    }
                    reader.Close();
                }
            }
            conn.Close();
            return uuid;
        }

        public void Remove(User user) {
            conn.Open();
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                DELETE FROM {ACCOUNTS}
                WHERE  username ='{user.Username}'";
                if (cmd.ExecuteNonQuery() > 0) {
                    Console.WriteLine(
                        $"{user.Username} was successfully removed."
                    );
                };
            }
            conn.Close();
        }

        public bool UserExists(User user) {
            conn.Open();
            bool userExists =  false;
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                SELECT username FROM {ACCOUNTS}
                WHERE username ='{user.Username}'
                ";
                using (var reader = cmd.ExecuteReader()) {
                    if (reader.HasRows) {
                        userExists = true;
                    }
                    reader.Close();
                }
            }
            conn.Close();
            return userExists;
        }
    }
}