using BLL.DTO;
using BLL.DTO.VM;
using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICacheService _cacheService;

        public OrderController(IOrderService orderService, ICacheService cacheService)
        {
            _orderService = orderService;
            _cacheService = cacheService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ShowOrderList")]
        [ResponseCache(CacheProfileName = "Caching")]
        public ActionResult<List<OrderVM>> ShowOrderList()
        {
            try
            {
                _cacheService.GetUserByIdAsync(1);
                var result = _orderService.ShowOrderList();

                return Ok(result);

                //throw new NotImplementedException();
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpDelete]
        [Route("DeleteOrder")]
        public ActionResult<Task> DeleteOrder(int orderId)
        {
            try
            {
                _orderService.DeleteOrder(orderId);
                return Ok("Success");
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("AddOrder")]
        public ActionResult<Task> AddOrder(VMAddOrder vMAddOrder)
        {
            try
            {
                OrderDTO someOrderDTO = AutoMapperService<VMAddOrder, OrderDTO>.Mapper(vMAddOrder);
                _orderService.AddOrder(someOrderDTO);
                return Ok("Success");
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
