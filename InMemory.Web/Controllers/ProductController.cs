using InMemory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemory.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            //ram'de belli bir keyde data var mı kontrolü için 2 yöntem vardır
            //1. yol
            //if (string.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            //{
            //    //key-value
            //    //zaman- datetime.now.tostring()
            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            //}
            //2.yol
            //if(!_memoryCache.TryGetValue("zaman", out string zaman))
            //{                
            //    //key-value
            //    //zaman- datetime.now.tostring()
            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            //}

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10); //10 saniyelik cacheleme
            //options.SlidingExpiration = TimeSpan.FromSeconds(30); //30 saniye içinde veriye eriştiğim sürece verinin ömrü 30 saniye uzayacaktır.

            options.Priority = CacheItemPriority.High;

            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set("callback", $"{key} -> {value} => sebep : {reason}");
            });

            //key-value
            //zaman- datetime.now.tostring()
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options);

            //complex veri cache ekleme
            Product product = new Product { Id = 1, Name = "Kalem", Price = 1 };
            _memoryCache.Set<Product>("product:1", product);

            return View();
        }

        public IActionResult Show()
        {
            //_memoryCache.Remove("zaman");

            //veriyi get etme yöntemleri:
            //1.yol : Get<T>(string key)
            ViewBag.zaman = _memoryCache.Get<string>("zaman");

            ////2.yol:  GetOrCreate<T>(string key, function)
            //_memoryCache.GetOrCreate<string>("zaman", entry =>
            //{
            //    return DateTime.Now.ToString();
            //});


            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.callback = callback;

            //complex veri cacheten çekme
            ViewBag.product = _memoryCache.Get<Product>("product:1");

            return View();
        }
    }
}
