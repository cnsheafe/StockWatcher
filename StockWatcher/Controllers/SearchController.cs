using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StockWatcher.Model;
using StockWatcher.Model.Schemas;
using StockWatcher.Model.Services;

namespace StockWatcher.Controllers
{
    /// <summary>
    /// Handles requests to lookup company names and stock symbols.
    /// </summary>
    public class SearchController : Controller
    {
        private readonly IQueryCompanyService service;
        private readonly StockDbContext context;
        public SearchController(StockDbContext _context)
        {
            context = _context;
            service = new QueryCompanyService(context);
        }

        /// <summary>
        /// Gets companies that match the query.
        /// </summary>
        /// <param name="query">
        /// A searchphrase.
        /// </param>
        /// <returns>
        /// Returns JSON of matching companies, if any
        /// </returns>
        [HttpGet("/company")]
        public IActionResult GetCompany([FromQuery]Query query)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
            JsonResult apiResult = Json(service.SearchCompanies(query));
            if (apiResult == null)
            {
                Response.StatusCode = 400;
                return null;
            }

            return apiResult;
        }
    }
}
