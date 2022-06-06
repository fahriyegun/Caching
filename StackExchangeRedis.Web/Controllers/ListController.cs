using Microsoft.AspNetCore.Mvc;
using StackExchangeRedis.Web.Services;

namespace StackExchangeRedis.Web.Controllers
{
    public class ListController : BaseController
    {
       private string listKey = "product";

        public ListController(RedisService redisService) : base(redisService)
        {
        }

        public IActionResult Index()
        {

            List<string> nameList = new List<string>();

            if (db.KeyExists(listKey))
            {
                db.ListRange(listKey).ToList().ForEach(x=> 
                {
                    nameList.Add(x.ToString());
                });
            }
            
            return View(nameList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            //right sona ekler.
            //left başa ekler.
            db.ListRightPush(listKey, name);
           
            return Redirect("Index");
        }
        
        public IActionResult Remove(string item)
        {
            db.ListRemoveAsync(listKey, item).Wait();
            
            //listenin başından siler.
            //db.ListLeftPop(listKey);
            return Redirect("Index");
        }
    }
}
