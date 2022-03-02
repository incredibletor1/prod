using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ScreenshotService : IScreenshotService
    {
        IGridFSBucket gridFS;
        IMongoCollection<Screenshot> Screenshots;
        public ScreenshotService()
        {
            string connectionString = "mongodb://localhost:27017/screenshotStore";
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient mongoClient = new MongoClient(connectionString);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(connection.DatabaseName);
            gridFS = new GridFSBucket(mongoDatabase);
            Screenshots = mongoDatabase.GetCollection<Screenshot>("Screenshots");
        }

        public async Task Create(IFormCollection s)
        {
            Screenshot screenshot = new Screenshot();
            screenshot.TimeSt = DateTime.Now;
            try
            {
                screenshot.ScreenShot = await gridFS.UploadFromStreamAsync("_" + screenshot.TimeSt, s.Files[0].OpenReadStream());
            } catch (Exception e)
            { 
                string a = e.Message;
            }
            await Screenshots.InsertOneAsync(screenshot);
             
        }
        public async Task<List<Screenshot>> Get()
        {
            var builder = new FilterDefinitionBuilder<Screenshot>();
            var filter = builder.Empty;
            return await Screenshots.Find(filter).ToListAsync();
        }
    }
}
