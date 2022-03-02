using DAL.Entities;
using DAL.HelpModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IOrderRepository<T> where T : class
    {
        void AddOrder(T t);

        void DeleteOrder(int orderId);

        List<T> ShowOrderList();

        int? DoSmth(int i)
        {
            return null;
        }

        Task SaveImage(byte[] bt);
    }
}
