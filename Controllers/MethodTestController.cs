using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StockWatcher.Model.Schemas;
using StockWatcher.Model.Actions;
using StockWatcher.Model.Data;

namespace StockWatcher.Controllers {
    public class MethodTestController: Controller {
        private SmsAction smsAction = new SmsAction();
        private StockRequestDb request = new StockRequestDb();

        [HttpPost]
        public void MakeBinding([FromBody]User user) {
            smsAction.MakeBinding(user);
        }

        [HttpGet]
        public void ListBindings() {
            smsAction.ListBindings();
        }

        [HttpPost]
        public void NotifyUsers([FromBody]JObject Data) {
            smsAction.NotifyUsers(
                Data["usernames"].ToObject<List<string>>(),
                Data["stock"].ToObject<Stock>(),
                Data["openPrice"].ToObject<double>()
            );
        }

        [HttpGet]
        public void AddUserRequest() {
            var stock = new Stock(){Username="user", Price=1, Equity="msft"};
            request.Add(stock);
        }

        [HttpGet]
        public void  GetUserRequest() {
            var stock = new Stock(){Username="user", Price=1, Equity="msft"};
            request.GetUsers(stock.RequestId);
        }

        [HttpGet]
        public void RemoveUserRequest() {
             var stock = new Stock(){Username="user", Price=1, Equity="msft"};
             request.Remove(stock.RequestId);
        }

    }
}