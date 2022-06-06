# Caching
.NET6 Caching (In-memory vs. Redis)

# Caching Nedir? 
Çok sık kullanılan dataların kaydedilme ve okunması şekline caching denir. Amaç uygulamayı daha hızlı çalıştırmak. 

# Caching Çeşitleri nelerdir?
1.	InMemory Caching(private)
    Uygulamaların datalarını, uygulamanın RAM’inden okuma işlemine denir. Önce ilgili data cachete varsa oradan okuyor, yoksa DB’den datayı çekip Ram’e kaydeder,sonraki okumalarda ramden veri çekilir. 

    Dezavantajları:
    * Tutulabilecek data miktarı uygulamanın host sisteminin sahip olduğu ram kadardır
    * Uygulama kaç tane sunucuda ayağa kaldırılıyorsa her bir sunucunun ram’ine datalar kaydedilir. Bu iki sunucuya farklı zamanlarda datalar ram’e kaydedilirse ve o zaman farkında datalar farklılaştıysa 2 sunucunun ram’inde farklı datalar tutulur. Veri uyuşmazlığı yaşanır.
    * Bunu engellemek için loadbalancerda istek yapıldığında hep aynı sunucudan veri çekilecek şekilde yönlendirebilir. Burada veri uyuşmazlığı yaşanmaz ama güncel veri görünmeyebilir. (sticky-session)
    
2.	Distributed Caching (shared)
    Shared cache service’de yani ortak kullanılabilecek ayrı bir serviste veri tutulur.
    
    Avantajları:
    * Veri tutarsızlığını önler.
    * Inmemory de sunucu restart olunca cachedeki datalarda silinecektir. Ama distributed cache olsa datalar silinmez.
    
    Dezavantajları:
    * Hızı, in-memory’e göre daha yavaştır. Inmemoryde hemen kendi raminden çekerken, distributed cache ortak servise sürekli istek atılır.
    * Implementasyonu daha zordur.
    
# Data ne zaman cachelenir?
* On-demand  caching :  talep olduğunda cachelenir. 
* Prepopulation caching : uygulama ayağa kalktığı anda db den çekip cachelenmesidir.

# Cache ömrü
* Absolute time : bir cache’e kesin bir ömür verilir (örn: 5 dk)
* Sliding time : bir verinin memory de ne kadar inactive kalacağını belirtiriz. Veriye ne kadar erişilirse verinin ömrü uzar. Eğer belirtilen süre kadar erişilmezse veri cacheten silinir.

# 1.	InMemory Cache
Kurulumu:
AddMemoryCache() IMemoryCache
* Servisi ayağa kaldırmak için program.cs’e bu servisi ekleriz.
-	builder.Services.AddMemoryCache();

* Kullanılacak controllerda yada classta interface’i oluşturur ve constructorda geçeriz.
 private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

Metotlar:
1.	Set : _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
2.	Get : _memoryCache.Get<string>("zaman");
3.	GetOrCreate:
 _memoryCache.GetOrCreate<string>("zaman", entry =>{return DateTime.Now.ToString();});
4.	Remove : _memoryCache.Remove("zaman");
5.	AbsoluteExpiration : 
options.AbsoluteExpiration = DateTime.Now.AddSeconds(30); //30 saniyelik cacheleme
                
6.	SlidingExpiration : 
options.SlidingExpiration = TimeSpan.FromSeconds(30); //30 saniye içinde veriye eriştiğim sürece verinin ömrü 30 saniye uzayacaktır.
                
7.	Priority : memory dolduğunda ve yeni bir data eklemeye çalıştığımız zaman ramden prioritye göre cacheten prioritysi düşük olan datayı siler.
options.Priority = CacheItemPriority.Low;

8.	RegisterPostEvictionCallback : memoryden bir data silindiğinde hangi sebepten silindiğini görebilmemizi sağlar. Slidingexpiration süresi dolduğu için mi, prioritysinden dolayı mı öğreniriz.
  options.RegisterPostEvictionCallback((key, value, reason, state) =>
  {
  _memoryCache.Set("callback", $"{key} ->{value} => sebep:{reason}");
  });


# 2.	Redis
Kurulumu:
Docker kurulumu yapılır.
Powershell açılır.
  * docker run -p 6379:6379 --name some-redis -d redis 
    bu komutla some-redis adında localimden 6379 portundan dockerdaki 6379 portuna erişim yapacak redisi ayağa kaldıracak komutu vermiş oluruz. Return value olarak da oluşan nesnenin containerid sini döner : b8663f860e46bd053f45306dd80108c5ebc974e6d5784aa47d438166c8fc2ae0
  * docker ps
    bununla dockerda ayakta olan containerleri listeler.
    CONTAINER ID   IMAGE     COMMAND                  CREATED         STATUS         PORTS                    NAMES
    b8663f860e46   redis     "docker-entrypoint.s…"   8 seconds ago   Up 7 seconds   0.0.0.0:6379->6379/tcp   some-redis
  * docker exec -it b86 sh # redis-cli
    yukarda oluşan containere (redis servera) interactive olarak bağlanırız. Redis-cli ile redis’in içine girmiş oluruz.(get add yapabiliriz) 
  * redis-cli –raw dersek Türkçe karakterleri destekler raw yazmazsak desteklemez.
  * 127.0.0.1:6379> ping 

Redis Veri Tipleri:
1.	String
- Powershell Komutları:
  * set name fahriye : OK
  * Get name : “fahriye”
  * Getrange name 0 2 :“fah”
  * Set ziyaretci 1000 :Ok
  * Incr ziyaretci :(integer) 1001
  * Incrby ziyaretci 10 : (integer) 1011
  * Decr ziyaretci : (integer) 1010
  * Decrby ziyaretci 10 : (integer) 1000
  * Append name cankaya
  

2.	List => LinkedList(c#)
- Powershell komutları
  * Lpush kitaplar kitap1 (LPUSH : Left push. listenin başına ekler)
  * Lpush kitaplar kitap2
  * Rpush kitaplar kitap3 (RPUSH: Right push. Listenin sonuna ekler) 
  * Lrange kitaplar 0 -1 (0. İndexten listede ne kadar data varsa gelsin istiyorsak -1, belli miktarda data gelsin istiyorsak -1 yerine onu yazarız. Listteki dataları çekeriz)
  * Lpop kitaplar (listenin başından kayıt siler)
  * Rpop kitaplar (listenin sonundan siler)
  * Lindex kitaplar 1 (kitaplar[1] elemanını döner)

3.	Set => HashSet(c#)
- Aynı datadan birden fazla eklenemez. List’de eklenebilir.
- Listenin random bir uyerine eklenir. List’de başına yada sonuna eklenebilir.
- Powershell komutları:
  * Sadd color blue : 1
  * Sadd color red : 1
  * Sadd color red : 0 (false döner eklenmez)
  * Smembers color : red blue
  * Srem color blue : 1
  

4.	Sorted Set : 
  - skorlara göre sıralama yapılır. Listedeki valuelar uniquetir ama skorlar unique olmak zorunda değil. Aynı valuedan farklı bir skorla ekleme yapılmak istendiğinde var olan valuenun scoreu güncellenir.
  - Powershell komutları:
  * Zadd kitaplar 1 kitap1
  * Zadd kitaplar 2 kitap2
  * Zrange kitaplar 0 -1
  * Zrange kitaplar 0 -1 withscores
  * Zrem kitaplar kitap1

5.	Hash => Dictionary(c#) : key	Value
  - Powershell komutları:
  * Hmset sozluk pen kalem
  * Hmset sozluk bag canta
  * Hget sozluk pen
  * Hdel sozluk pen
  * Hgetall sozluk

Kod tarafında kullanımı
  1. yöntem :
  - Manage nuget packages den Microsoft.Extensions.Caching.StackExchangeRedis indirilir.
  - Servisi ayağa kaldırmak için program.cs’e bu servisi ekleriz.
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379";
    });

  - Kullanılacak controllerda yada classta interface’i oluşturur ve constructorda geçeriz.
    private IDistributedCache _distributedCache;
    public ProductsController(IDistributedCache distributedCache)
    {
        distributedCache = _distributedCache;
    }
  
  Metotları:
  * Set : _distributedCache.SetString("name", "fahriye");
  *	Get: string name = _distributedCache.GetString("name");
  * Remove: _distributedCache.Remove("name");
  * Set complex data:
  Product product = new Product { Id = 1, Name= "Kalem", Price=10 };
  string jsonProduct = JsonConvert.SerializeObject(product);
  _distributedCache.SetString("product:1", jsonProduct);
  
  Get complex data:
  string jsonProduct = _distributedCache.GetString("product:1");
  product = JsonConvert.DeserializeObject<Product>(jsonProduct);  
  
  Set as byte:
  Byte[] byteproduct = Encoding.UTF8.GetBytes(jsonProduct); _distributedCache.Set("product:1", byteproduct);
  
  Get as byte:
  Byte[] byteProduct = _distributedCache.Get("product:1");
  jsonProduct = Encoding.UTF8.GetString(byteProduct);
  product = JsonConvert.DeserializeObject<Product>(jsonProduct);


* Microsoft.Extensions.Caching.StackExchangeRedis ile sadece string yada byte olarak get set yapılır. Daha fazla veri tipleriyle işlemler yapılmaz.

2.yöntem :
  - StackExchnge.Redis  manage nuget package dan indirilir.
  - App.settinge erişmek istediğimiz adresi yazarız. Port yazmaya gerek yok default 6379 alır. 
  "ConnectionStrings": 
  {
    "Redis": "localhost"
  }
  - Redis server’a bağlanmak için program.cs de configler ayağa kaldırılır:
  IConfiguration configuration = builder.Configuration;
  var multiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
  builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer); 
  
  - Ara bir redis service classı oluştururuz burada hangi db ye bağlanacağımızı falan söyleyebiliriz.
  - Bundan sonra hangi sınıfta bu redisservice classını kullanmak istiyorsak constructorda geçeriz ve powershellde yazılabilecek tüm işlemler burada yapılır.
  - Redis’te slidingexpiration özelliği yok, absoluteexpiration var. Ama sliding özelliği vermek istiyorsak veriye her istek yapıldıkta absolutetime’ı güncellersek aynı işlevi görecektir. Örneğin:
  if (!db.KeyExists(keyName))
  {
    db.KeyExpire(keyName, DateTime.Now.AddMinutes(5));
  }





