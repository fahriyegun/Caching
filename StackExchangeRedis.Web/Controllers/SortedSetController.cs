using Microsoft.AspNetCore.Mvc;
using StackExchangeRedis.Web.Services;

namespace StackExchangeRedis.Web.Controllers
{
    public class SortedSetController : BaseController
    {
       private string keyName = "student";

        public SortedSetController(RedisService redisService) : base(redisService)
        {
        }

        public IActionResult Index()
        {

            HashSet<string> customers = new HashSet<string>();

            if (db.KeyExists(keyName))
            {
                db.SortedSetScan(keyName).ToList().ForEach(x =>
                {
                    customers.Add(x.ToString());
                });
            }

            //descending order
            //db.SortedSetRangeByRank(keyName, order:Order.Descending).ToList().ForEach(x =>
            //{
            //    customers.Add(x.ToString());
            //});
        
            return View(customers);
        }

        [HttpPost]
        public IActionResult Add(string item, int score)
        {
            db.SortedSetAdd(keyName, item, score);

            return Redirect("Index");
        }

        public IActionResult Remove(string item)
        {
            db.SortedSetRemove(keyName, item);

            return Redirect("Index");
        }
    }
}
