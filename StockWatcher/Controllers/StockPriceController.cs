using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using StockWatcher.Model.Services;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Controllers
{
    /// <summary>
    /// Handles requests for fetching stockprices
    /// </summary>
    public class StockPriceController : Controller
    {
        private AlphaVantage service;
        public StockPriceController(AlphaVantage context)
        {
            service = context;
        }

        /// <summary>
        /// Fetches stockprices from AlphaVantage based on companies.
        /// </summary>
        /// <param name="listOfSymbols">
        /// JSON of company names to fetch prices for.
        /// </param>
        /// <returns>
        /// JSON of time-series data of each company or null on AV-side error.
        /// </returns>
        [HttpPost]
        [Route("/stockprice")]
        public IActionResult Index([FromBody]ListOfSymbols listOfSymbols)
        {
            var data = service.RequestStockPrices(
                listOfSymbols.Symbols, TimeSeries.Intraday, IntervalTypes.OneMinute
            );
            return Json(data.Result);
        }

        /// <summary>
        /// CORS Pre-flight since incoming data is JSON.
        /// </summary>
        [HttpOptions]
        [Route("/stockprice")]
        public void Options()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "https://stock-watcher-client.herokuapp.com");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST");
        }

    }

}