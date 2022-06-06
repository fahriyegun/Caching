using Microsoft.AspNetCore.Mvc;
using StackExchangeRedis.Web.Services;

namespace StackExchangeRedis.Web.Controllers
{
    public class StringController : BaseController
    {
        public StringController(RedisService redisService) : base(redisService)
        {
        }

        public IActionResult Index()
        {
            db.StringSet("name", "fahriye");
            db.StringSet("ziyaretci", 100);

            return View();
        }

        public IActionResult Show()
        {
            var name = db.StringGet("name");

            if (name.HasValue)
            {
                ViewBag.name = name.ToString();
            }

            var ziyaretci = db.StringGet("ziyaretci");
            ViewBag.ziyaretci = ziyaretci;

            var nameSubStr = db.StringGetRange("name", 0, 3);

            db.StringDecrement("ziyaretci",10);

            //async kullanımları
            //wait() ile async olarak metot çalışır  ve sonucunun ne olduğunu çekmemize ihtiyaç yoktur
            //result ile async olarak metot çalışır ve sonucunu çekmemiz beklenir.
            //db.StringGetAsync("name").Wait();
            //var result = db.StringGetAsync("name").Result;

            return View();
        }
    }
}
