using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using StockWatcher.Model.Services;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Controllers
{
    public class StockPriceController : Controller
    {
        private AlphaVantage service;
        public StockPriceController(AlphaVantage context)
        {
            service = context;
        }

        [HttpPost]
        [Route("stockprice")]
        public IActionResult Index([FromBody]ListOfSymbols listOfSymbols)
        {
            var data = service.RequestStockPrices(
                listOfSymbols.Symbols, TimeSeries.Intraday, IntervalTypes.OneMinute
            );
            return Json(data.Result);
        }
    }

}