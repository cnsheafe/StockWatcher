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

        private const string TABLENAME = "ACCOUNTS";
        /// <summary>
        /// Adds a user to the table
        /// </summary>
        /// <param name="user">User info including phone number</param>
        /// <para/>
        /// <returns>True if user was added to the table. Otherwise False.</returns>
        public bool Add(User user) {
            user.Uuid = Guid.NewGuid().ToString();
            conn.Open();
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = $@"
                INSERT INTO {TABLENAME} (username, password, phone, uuid)
                VALUES (
                    '{user.Username}',
                    '{user.Password}',
                    '{user.Phone}',
                    '{user.Uuid}'
                )
                ON CONFLICT (username)
                DO NOTHING";

                if (cmd.ExecuteNonQuery() == 0) {
                    conn.Close();
                    return false;
                }
                conn.Close();
                return true;
            }
        }
        public string GetUuid(string username) {
        conn.Open();
        string uuid = "";
        using (var cmd = new NpgsqlCommand()) {
            cmd.Connection = conn;
            cmd.CommandText = $@"
            SELECT uuid FROM {TABLENAME}
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