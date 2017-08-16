using System;

using StockWatcher.Model.Data;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Actions {
    public static class ManageAccount {
        public static bool AddUser(User user) {
            var account = new AccountsDb();
            var smsAction = new SmsAction();
            bool success = false;
            if (!account.UserExists(user)) {
                account.Add(user);
                smsAction.MakeBinding(user);
                success = true;
            }
            return success;
        }

        private static void CheckInput(User user) {
        }
    }
}