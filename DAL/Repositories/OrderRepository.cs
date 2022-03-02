using DAL.Context;
using DAL.Entities;
using DAL.HelpModels;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class OrderRepository : IOrderRepository<Order>
    {
        private readonly DatabaseContext _context;

        public OrderRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task SaveImage(byte[] bt)
        {
            _context.ProductImages.Add(new ProductImage { ByteImage = bt });
        }

        public int? DoSmth(int i)
        {
            return 1;
        }

        public void AddOrder(Order order)
        {
            if (order != null)
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
            } 
            else throw new NullReferenceException("ERROR : Order null");
        }

        public void DeleteOrder(int orderId)
        {
            if (orderId != 0)
            {
                Order someOrder = _context.Orders.FirstOrDefault(p => p.Id == orderId);
                if (someOrder != null)
                {
                    _context.Entry(someOrder).State = EntityState.Deleted;
                    _context.SaveChanges();
                }
                else throw new NullReferenceException($"ERROR : No id = {orderId}");
            }
            else throw new NullReferenceException("ERROR : No id = 0");
        }

        public List<Order> ShowOrderList()
        {
            var someOrders = _context.Orders.ToList();
            if (someOrders != null)
            {
                return someOrders;
            }
            else throw new NullReferenceException("ERROR : No orders");

            //throw new NotImplementedException();
        }
    }
}
