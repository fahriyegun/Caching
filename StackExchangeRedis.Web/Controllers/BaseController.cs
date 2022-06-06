using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using StackExchangeRedis.Web.Services;

namespace StackExchangeRedis.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly RedisService _redisService;
        protected readonly IDatabase db;
        public BaseController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(1);
        }

       
    }
}
