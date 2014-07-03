using System.Collections.Generic;
using System.Linq;
using AbacusAPI.Models;
using AbacusAPI.Repositories;

namespace AbacusAPI
{
    public class OrdersService : IOrdersService
    {
        public IEnumerable<OrderInfo> GetAllOrders()
        {
            var repo = new OrderInfoRepository();
            return repo.GetAllOrders().ToList();
        }

        public IEnumerable<OrderInfo> GetOrdersNoSchoolCode()
        {
            var repo = new OrderInfoRepository();
            return repo.GetOrdersNoSchoolCode().ToList();
        }

        public IEnumerable<OrderInfo> GetOrdersSchoolsNotActive()
        {
            var repo = new OrderInfoRepository();
            return repo.GetOrdersSchoolsNotActive();
        }

        public IEnumerable<OrderInfo> GetOrdersZeroQuantities()
        {
            var repo = new OrderInfoRepository();
            return repo.GetOrdersZeroQuantities();
        }

        public IEnumerable<OrderInfo> GetOrdersGoodAndRed()
        {
            var repo = new OrderInfoRepository();
            return repo.GetOrdersGoodAndRed();
        }

        public IEnumerable<OrderInfo> GetOrdersGoodAndYellow()
        {
            var repo = new OrderInfoRepository();
            return repo.GetOrdersGoodAndYellow();
        }

        public IEnumerable<OrderInfo> SearchOrders(string searchText)
        {
            var repo = new OrderInfoRepository();
            return repo.SearchOrders(searchText);
        }

        public void AssignSchoolToOrder(School school, OrderInfo order)
        {
            var repo = new OrderInfoRepository();
            repo.AssignSchoolToOrder(school, order);
        }

        public void EditOrder(OrderInfo order)
        {
            var repo = new OrderInfoRepository();
            repo.EditOrder(order);
        }

        public IEnumerable<OrderChangeLog> GetOrderChangeHistory()
        {
            var repo = new OrderInfoRepository();
            return repo.GetOrderChangeHistory();
        }
    }
}
