using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.VM
{
    public class ProctoringImage
    {
        public int Id { get; set; }
        //public ImageType ImageTypeId { get; set; }
        public DateTime ImageTimestamp { get; set; }
        public string FileSystemPath { get; set; }
        public int Order { get; set; }
        public int UserExamSessionId { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
        public string UserDelete { get; set; }
        public DateTime DateDelete { get; set; }
       // public byte[] ImageData { get; set; }
       // public IFormFile Image { get; set; }
    }

    public enum ImageType
    {
        proctoring = 1,
        user_ID = 2,
        user_image = 3
    }
}
