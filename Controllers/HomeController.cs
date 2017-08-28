using System;
using Microsoft.AspNetCore.Mvc;

namespace StockWatcher.Controllers 
{
    public class HomeController: Controller 
    {

        [HttpGet]
        public IActionResult Home() 
        {
            string UserId;
            if (Request.Cookies.TryGetValue("stock-watcher", out UserId)) 
            {

            }
            return View();
        }
    }
}