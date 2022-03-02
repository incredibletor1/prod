using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddOrder(OrderDTO orderDTO)
        {
            Order someOrder = AutoMapperService<OrderDTO, Order>.Mapper(orderDTO);
            _unitOfWork.Orders.AddOrder(someOrder);
        }

        public async Task DeleteOrder(int orderId)
        {
            _unitOfWork.Orders.DeleteOrder(orderId);
        }

        public List<OrderDTO> ShowOrderList()
        {
            
            return AutoMapperService<Order, OrderDTO>.MapperList(_unitOfWork.Orders.ShowOrderList());

            //throw new NotImplementedException();
        }
    }
}
