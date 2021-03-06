﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using AbacusAPI.Models;

namespace AbacusAPI
{
    [ServiceContract]
    public interface IOrdersService
    {
        [OperationContract]
        IEnumerable<OrderInfo> GetAllOrders();

        [OperationContract]
        IEnumerable<OrderInfo> GetOrdersNoSchoolCode();

        [OperationContract]
        IEnumerable<OrderInfo> GetOrdersSchoolsNotActive();

        [OperationContract]
        IEnumerable<OrderInfo> GetOrdersZeroQuantities();

        [OperationContract]
        IEnumerable<OrderInfo> GetOrdersGoodAndRed();

        [OperationContract]
        IEnumerable<OrderInfo> GetOrdersGoodAndYellow();

        [OperationContract]
        IEnumerable<OrderInfo> GetOrdersWithNoPicture();
        
        [OperationContract]
        IEnumerable<OrderInfo> SearchOrders(string searchText);

        [OperationContract]
        void AssignSchoolToOrder(School school, OrderInfo order);

        [OperationContract]
        void EditOrder(OrderInfo order);

        [OperationContract]
        IEnumerable<OrderChangeLog> GetOrderChangeHistory();
    }
}
