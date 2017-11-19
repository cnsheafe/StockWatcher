using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using StockWatcher.Model.Services;

namespace StockWatcher.Controllers
{
    public class StockPriceController : Controller
    {
        private AlphaVantage service;
        public StockPriceController(AlphaVantage context)
        {
            service = context;
        }

        [HttpGet]
        [Route("stockprice")]
        public IActionResult Index([FromQuery]string StockSymbol)
        {
            var data = service.RequestStockPrice(StockSymbol, TimeSeries.Intraday, IntervalTypes.OneMinute);
            return Json(data.Result);
        }
    }

}