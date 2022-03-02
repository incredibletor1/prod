using DAL.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IScreenshotService
    {
        Task<List<Screenshot>> Get();

        Task Create(IFormCollection s);
    }
}
