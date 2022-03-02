using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prod.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace prod.Controllers
{

    [ServiceFilter(typeof(ExceptionFilter))]
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DynamoDbController : ControllerBase
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        private static readonly string _tablename = "Product";
        //
        private readonly IAmazonS3 _awsS3Client;

        private static readonly string _bucketName = "product-bucket1";

        public DynamoDbController(IAmazonDynamoDB amazonDynamoDB, IAmazonS3 amazonS3)
        {
            _amazonDynamoDB = amazonDynamoDB;
            _awsS3Client = amazonS3;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Post")]
        public async Task<IActionResult> Post(Product product)
        {
            var data = new Dictionary<string, AttributeValue>();
            data.Add("_id", new AttributeValue { S = product._id });
            data.Add("Name", new AttributeValue { S = product.Name });
            data.Add("Cost", new AttributeValue { S = product.Cost.ToString() });
            data.Add("Form", new AttributeValue { S = "Blade"});

            await _amazonDynamoDB.PutItemAsync(_tablename, data); 
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            var conditions = new List<ScanCondition>();
            var result = await _amazonDynamoDB.ScanAsync(_tablename, new List<string>());
            result.Items.ToList();

            return Ok(result.Items.ToList());
        }

        [HttpGet]
        [Route("GetFromBucket")]
        public async Task<IActionResult> GetFromBucker(string productId)
        {
            await _awsS3Client.DownloadToFilePathAsync(_bucketName, $"product{productId}.txt", $"D:/OtherStuff/product{productId}.txt", new Dictionary<string, object>());
            return Ok();
        }

        [HttpPost]
        [Route("UploadToBucket")]
        public async Task<IActionResult> UploadToBucket(string productId, string data)
        {
            FileStream fileStream = System.IO.File.Open($"D:/OtherStuff/ForUpload/product{productId}.txt", FileMode.Create);
            fileStream.Close();

            using (StreamWriter outputFile = new StreamWriter(Path.Combine($"D:/OtherStuff/ForUpload/product{productId}.txt")))
            {
                await outputFile.WriteLineAsync(data);
                outputFile.Close();
            }

            await _awsS3Client.UploadObjectFromFilePathAsync(_bucketName, $"product{productId}.txt", $"D:/OtherStuff/ForUpload/product{productId}.txt", new Dictionary<string, object>());
            return Ok();
        }
    }
}
