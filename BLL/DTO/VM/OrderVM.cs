﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.VM
{
    public class OrderVM
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public byte[] ProductImage { get; set; }
    }
}
