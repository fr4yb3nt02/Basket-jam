using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        public IMongoDatabase MongoDatabase { get; set; }
        public DataContext(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
        }

            public DataContext(DbContextOptions options) : base(options)
    {
    }
    }
}