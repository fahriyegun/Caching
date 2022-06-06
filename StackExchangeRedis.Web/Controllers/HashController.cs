using Microsoft.AspNetCore.Mvc;
using StackExchangeRedis.Web.Services;

namespace StackExchangeRedis.Web.Controllers
{
    public class HashController : BaseController
    {
        private string keyName = "country";
        public HashController(RedisService redisService) : base(redisService)
        {
        }

        public IActionResult Index()
        {

            Dictionary<string, string> countries = new Dictionary<string, string>();

            if (db.KeyExists(keyName))
            {
                db.HashGetAll(keyName).ToList().ForEach(x =>
                {
                    countries.Add(x.Name.ToString(), x.Value.ToString());
                });
            }


            return View(countries);
        }

        [HttpPost]
        public IActionResult Add(string name, string value)
        {
            db.HashSet(keyName, name, value);

            return Redirect("Index");
        }

        public IActionResult Remove(string item)
        {
            db.SortedSetRemove(keyName, item);

            return Redirect("Index");
        }
    }
}
