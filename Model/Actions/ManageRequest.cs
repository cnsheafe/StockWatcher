using System;

using Hangfire;

using StockWatcher.Model.Schemas;
using StockWatcher.Model.Data;

namespace StockWatcher.Model.Actions {
    public static class ManageRequest {
        public static bool Add(Stock stock) {
            var request = new StockRequestDb();
            if (request.IsRunning(stock.RequestId)) {
                return false;
            }

            request.Add(stock);
            Console.WriteLine("Request was added");
            return true;
        }

        public static bool Remove(string requestId) {
            var request = new StockRequestDb();
            if (request.IsRunning(requestId)) {
                request.Remove(requestId);
            }
            return !request.IsRunning(requestId) ? true : false;
        }
    }
}