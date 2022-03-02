using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prod.Controllers
{
    public class CacheController : ControllerBase
    {
        private readonly IDatabase database;
        
        public CacheController(IDatabase database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<string> Get(string key)
        {
            return await database.StringGetAsync(key);
        }

        [HttpPost]
        [Route("Post")]
        public async Task Post(string key, string value)
        {
            await database.StringSetAsync(key, value);
        }
    }
}
