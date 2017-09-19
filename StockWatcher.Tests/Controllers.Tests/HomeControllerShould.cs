using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace StockWatcher.Controllers.Tests
{
    public class HomeControllerShould
    {
            
        [Fact]
        public void ReturnDefaultView()
        {
            var result = new HomeController().Index();
            Assert.IsType<ViewResult>(result);
        }

    }
}