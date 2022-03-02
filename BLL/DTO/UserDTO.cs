using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class UserDTO
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
