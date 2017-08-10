using System;

using Hangfire;

using StockWatcher.Model.Schemas;
using StockWatcher.Model.Data;

namespace StockWatcher.Model.Actions {
    public static class QueueRequest {
        public static bool Add(Stock stock) {
            var request = new StockRequestDb();
            if (request.IsRunning(stock.RequestId))
                return false;
            request.Add(stock);
            return true;
        }

        public static void Remove(string requestId) {
            var request = new StockRequestDb();
            if (request.IsRunning(requestId))
                request.Remove(requestId);
        }
    }
}