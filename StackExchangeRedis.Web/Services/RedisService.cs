using StackExchange.Redis;

namespace StackExchangeRedis.Web.Services
{
    public class RedisService
    {
        public IDatabase _cache { get; set; }
        private IConnectionMultiplexer _redisCon;

        public RedisService(IConnectionMultiplexer redisCon)
        {
            _redisCon = redisCon;
            _cache = redisCon.GetDatabase();
        }

        public IDatabase GetDb(int id)
        {
            return _redisCon.GetDatabase(id);
        }
    }
}
