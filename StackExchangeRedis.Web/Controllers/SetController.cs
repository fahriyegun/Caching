using Microsoft.AspNetCore.Mvc;
using StackExchangeRedis.Web.Services;

namespace StackExchangeRedis.Web.Controllers
{
    public class SetController : BaseController
    {

        private string keyName = "customer";

        public SetController(RedisService redisService) : base(redisService)
        {
        }

        public IActionResult Index()
        {
            HashSet<string> customers = new HashSet<string>();

            if (db.KeyExists(keyName))
            {
                db.SetMembers(keyName).ToList().ForEach(x=>
                {
                    customers.Add(x);
                });
            }
            return View(customers);
        }

        [HttpPost]
        public IActionResult Add(string item)
        {
            //sliding expiration 
            if (!db.KeyExists(keyName))
            {
                db.KeyExpire(keyName, DateTime.Now.AddMinutes(5));

            }
            db.SetAdd(keyName, item);

            return Redirect("Index");
        }

        public async Task<IActionResult> Remove(string item)
        {
            await db.SetRemoveAsync(keyName, item);

            return Redirect("Index");
        }
    }
}
