using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StockWatcher.Model.Schemas;
using StockWatcher.Model.Services;

namespace StockWatcher.Controllers
{
    public class SearchController: Controller
    {
        private readonly QueryCompanyService service;
        public SearchController(QueryCompanyService _service)
        {
            service = _service;
        }

        [HttpGet("/company")]
        public IActionResult GetCompany()
        {
            var queryTerms = Request.Query;
            Query query = new Query();

            IEnumerable<PropertyInfo> allowedKeys = typeof(Query).GetProperties();

            foreach (var item in allowedKeys)
            {
                var key = item.Name.ToString();
                if (!queryTerms.ContainsKey(key))
                {
                    Response.StatusCode = 400;
                    return Json("Error");
                }
            }    

            StringValues tmp;
            queryTerms.TryGetValue("searchphrase", out tmp);
            query.SearchPhrase = tmp;
            queryTerms.TryGetValue("issymbol", out tmp);
            query.IsSymbol = tmp;
            Response.StatusCode = 200;
            JsonResult apiResult = Json(service.SearchCompanies(query));

            return apiResult;
        }
    }
}
