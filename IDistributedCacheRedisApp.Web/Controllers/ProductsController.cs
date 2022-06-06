using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private IDistributedCache _distributedCache;

        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public IActionResult Index()
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

            //primitive data type caching
            _distributedCache.SetString("name", "fahriye");

            //complex data type caching as a string
            Product product = new Product { Id = 1, Name= "Kalem", Price=10 };
            string jsonProduct = JsonConvert.SerializeObject(product);
            _distributedCache.SetString("product:1", jsonProduct);

            ////complex data type caching as a byte array
            //Byte[] byteproduct = Encoding.UTF8.GetBytes(jsonProduct);
            //_distributedCache.Set("product:1", byteproduct);

            return View();
        }

        public IActionResult Show()
        {
            string name = _distributedCache.GetString("name");
            ViewBag.name = name;

            //String
            string jsonProduct = _distributedCache.GetString("product:1");
            Product product = JsonConvert.DeserializeObject<Product>(jsonProduct);  
            ViewBag.product = product;

            //BYTE array
            //Byte[] byteProduct = _distributedCache.Get("product:1");
            //jsonProduct = Encoding.UTF8.GetString(byteProduct);
            //product = JsonConvert.DeserializeObject<Product>(jsonProduct);


            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("name");
            return View();
        }
    }
}
