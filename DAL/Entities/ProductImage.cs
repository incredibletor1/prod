using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public byte[] ByteImage { get; set; }

        //
        /*public Product Product { get; set; }
        public int ProductId { get; set; }*/
    }
}