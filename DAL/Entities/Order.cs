using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }

        //
        public ICollection<Product> Products { get; set; }
        public Order()
        {
            Products = new List<Product>();
        }
        /*public User User { get; set; }
        public string UserId { get; set; }*/
    }
}
