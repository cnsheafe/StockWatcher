using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using StockWatcher.Model.Services;

namespace StockWatcher.Controllers
{
    public class StockPriceController : Controller
    {
        private IAlphaVantageService service;
        public StockPriceController(IAlphaVantageService _service)
        {
            service = _service;
        }

        [HttpGet]
        [Route("stockprice")]
        public IActionResult Index([FromQuery]string StockSymbol)
        {
            var data = service.RequestStockPrice(StockSymbol);
            return Json(data.Result);
        }
    }

}