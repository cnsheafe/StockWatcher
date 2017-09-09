using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using StockWatcher.Model.Services;

namespace StockWatcher.Controllers
{
    public class StockPriceController : Controller
    {
        private AlphaVantageService service;
        public StockPriceController(AlphaVantageService _service)
        {
            service = _service;
        }
        public IActionResult Index([FromQuery]string StockSymbol)
        {
            var data = service.RequestStockPrice(StockSymbol);
            return Json(data.Result);
        }
    }

}