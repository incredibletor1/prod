using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class User 
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<Order> Orders { get; set; }
        public User()
        {
            Orders = new List<Order>();
        }
    }
}
