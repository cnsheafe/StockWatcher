using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockWatcher.Model.Services
{
    public class AlphaVantageService
    {
            public async Task<string> RequestStockPrice(string StockSymbol)
            {
                using (var client = new HttpClient())
                {
                    string responseBody = "";
                    string AV_KEY = Environment.GetEnvironmentVariable("AV_KEY");

                    // Describes URI query terms for Alphavantage API
                    var queryTerms = new StringBuilder();
                    queryTerms.Append("function=time_series_intraday&");
                    queryTerms.Append($"symbol={StockSymbol}&");
                    queryTerms.Append("interval=1min&");
                    queryTerms.Append($"apikey={AV_KEY}");

                    var avUri = new UriBuilder();
                    avUri.Scheme = "https";
                    avUri.Host = "www.alphavantage.co";
                    avUri.Path = "query";
                    avUri.Query = queryTerms.ToString();

                    // Asynchronous HTTP call
                    try 
                    {
                        responseBody = await client.GetStringAsync(avUri.Uri);
                        Console.WriteLine("Success!");
                    }
                    catch(HttpRequestException err) 
                    {
                        Console.WriteLine("\n Exception Caught!");
                        Console.WriteLine($"Message: {err.Message}");
                    }

                    return responseBody;
                }
            }
    }
}