using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Screenshot
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int SenderId { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime TimeSt { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ScreenShot { get; set; }
        IFormFile UploadedFile { get; set; }
    }
}
