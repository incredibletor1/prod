using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [DynamoDBTable("Product")]
    public class Product
    {
        [DynamoDBHashKey]
        public string _id { get; set; }

        [DynamoDBRangeKey]
        public string Name { get; set; }

        public int Cost { get; set; }

        //
        /*public Order Order { get; set; }
        public int OrderId { get; set; }*/

        /*public ICollection<ProductImage> ProductImages { get; set; }
        public Product()
        {
            ProductImages = new List<ProductImage>();
        }*/
    }
}
