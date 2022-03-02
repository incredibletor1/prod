using BLL.DTO.VM;
using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prod.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace prod.Controllers
{
    [ServiceFilter(typeof(ExceptionFilter))]
    [ApiController]
    [Route("[controller]")]
    public class ScreenshotController : ControllerBase
    {
        private readonly IScreenshotService _screenshotService;

        public ScreenshotController(IScreenshotService screenshotService)
        {
            _screenshotService = screenshotService;
        }

        [AllowAnonymous]
        [HttpGet]
        public Task<List<Screenshot>> Get()
        {
            return _screenshotService.Get();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<Task> Create(ProctoringImage proctoringImage)
        {
            try
            {
                //_screenshotService.Create(proctoringImage.Image);
            }
            catch(Exception e) { return NotFound(); }
            return Ok();
        }
    }
}
