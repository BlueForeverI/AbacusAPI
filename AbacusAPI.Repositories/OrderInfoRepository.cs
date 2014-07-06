using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AbacusAPI.Models;
using AbacusAPI.DataContext;

namespace AbacusAPI.Repositories
{
    public class OrderInfoRepository
    {
        private IQueryable<OrderInfo> GetOrders(string condition)
        {
            using (var context = new AbacusEntities())
            {
                var result = new List<OrderInfo>();
                var abacusConnection = new SqlConnection(context.Database.Connection.ConnectionString);
                abacusConnection.Open();
                string query = String.Format(GetOrderInfoQuery, condition);
                var command = new SqlCommand(query, abacusConnection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OrderInfo currentOrderInfo = GetOrderInfoFromReader(reader);
                    if (currentOrderInfo != null)
                    {
                        result.Add(currentOrderInfo);
                    }
                }

                return result.AsQueryable();
            }
        }

        public IQueryable<OrderInfo> GetAllOrders()
        {
            return GetOrders("1 = 1");
        }

        public IQueryable<OrderInfo> GetOrdersNoSchoolCode()
        {
            return GetAllOrders().Where(o => o.Code == "0");
        }

        public IQueryable<OrderInfo> GetOrdersSchoolsNotActive()
        {
            return GetAllOrders().Where(o => o.CustomerOrderStatus == "No Paper")
                .Where(o => !(o.Calcs == 0 && o.Cards == 0 && o.Diary == 0));
        }

        public IQueryable<OrderInfo> GetOrdersZeroQuantities()
        {
            return GetOrders("oi.Quantity = 0");
        }

        public IQueryable<OrderInfo> GetOrdersGoodAndRed()
        {
            return GetAllOrders()
                .Where(o => o.Status == "Good" && o.CustomerOrderStatus == "Has been invoiced")
                .Where(o => !(o.Calcs == 0 && o.Cards == 0 && o.Diary == 0));
        }

        public IQueryable<OrderInfo> GetOrdersGoodAndYellow()
        {

            return GetAllOrders()
                .Where(o => o.Status == "Good" && o.CustomerOrderStatus == "Has ordered paper")
                .Where(o => !(o.Calcs == 0 && o.Cards == 0 && o.Diary == 0));
        }


        public IQueryable<OrderInfo> GetOrdersWithNoPicture()
        {
            return GetOrders("oi.Id IS NULL");
        }

        public IEnumerable<OrderInfo> SearchOrders(string searchText)
        {
            return GetAllOrders()
                .Where(o =>
                    o.Status.ToLower().Contains(searchText) ||
                    o.Email.ToLower().Contains(searchText) ||
                    o.Name.ToLower().Contains(searchText) ||
                    o.Phone.ToLower().Contains(searchText) ||
                    o.Code.ToLower().Contains(searchText) ||
                    o.School.ToLower().Contains(searchText) ||
                    o.Address.ToLower().Contains(searchText) ||
                    o.KidsName.ToLower().Contains(searchText) ||
                    o.Room.ToLower().Contains(searchText) ||
                    o.Comment.ToLower().Contains(searchText) ||
                    o.Country.ToLower().Contains(searchText));
        }

        public void AssignSchoolToOrder(School school, OrderInfo order)
        {
            using(var context = new AbacusEntities())
            {
                utSPOrder orderEntity = context.utSPOrders.SingleOrDefault(o => o.Id == order.SPOrderId);
                OrdersChangeLog logEntry = new OrdersChangeLog();
                logEntry.Id = Guid.NewGuid();
                logEntry.ImageId = order.ImageId;
                logEntry.OrderType = order.OrderType;

                int quantity = (order.OrderType == "Calendars")
                                   ? (int) order.Calcs
                                   : ((order.OrderType == "Cards" ? (int)order.Cards : (int)order.Diary));

                logEntry.OldQuantity = quantity;
                logEntry.NewQuantity = quantity;
                logEntry.OldRoom = order.Room;
                logEntry.NewRoom = order.Room;
                logEntry.OldSchoolCode = orderEntity.SchoolCode ?? 0;
                logEntry.NewSchoolCode = (int) school.Code;

                orderEntity.SchoolCode = (int)school.Code;
                orderEntity.OrganisationName = school.Name;
                context.SaveChanges();

                InsertChangeLogEntry(logEntry);
            }
        }

        public void EditOrder(OrderInfo order)
        {
            using (var context = new AbacusEntities())
            {
                utOrderItem orderItem = context.utOrderItems.SingleOrDefault(o => o.Id == order.OrderItemId);
                utSPOrder spOrder = context.utSPOrders.SingleOrDefault(o => o.Id == order.SPOrderId);
                OrdersChangeLog logEntry = new OrdersChangeLog();
                logEntry.Id = Guid.NewGuid();
                logEntry.OldQuantity = orderItem.Quantity;
                logEntry.OrderType = orderItem.ItemType;
                logEntry.ImageId = (int)order.ImageId;
                logEntry.OldSchoolCode = int.Parse(order.Code);
                logEntry.NewSchoolCode = int.Parse(order.Code);

                if (orderItem.ItemType == "Cards")
                {
                    orderItem.Quantity = (int)order.Cards;
                }

                if (orderItem.ItemType == "Calendars")
                {
                    orderItem.Quantity = (int)order.Calcs;
                }

                if (orderItem.ItemType == "Diaries")
                {
                    orderItem.Quantity = (int)order.Diary;
                }

                logEntry.NewQuantity = orderItem.Quantity;
                logEntry.OldRoom = spOrder.StudentClass;
                logEntry.NewRoom = order.Room;

                InsertChangeLogEntry(logEntry);
                
                spOrder.StudentClass = order.Room;

                context.SaveChanges();
            }
        }

        private void InsertChangeLogEntry(OrdersChangeLog entry)
        {
            using(var context = new AbacusEntities())
            {
                context.OrdersChangeLogs.Add(entry);
                context.SaveChanges();
            }
        }

        private OrderInfo GetOrderInfoFromReader(SqlDataReader reader)
        {
            decimal qtyCard = (reader["QtyCard"].ToString() == "") ? 0 : decimal.Parse(reader["QtyCard"].ToString());
            decimal qtyCalendars = (reader["QtyCalendar"].ToString() == "") ? 0 : decimal.Parse(reader["QtyCalendar"].ToString());
            decimal qtyDiaries = (reader["QtyDiary"].ToString() == "") ? 0 : decimal.Parse(reader["QtyDiary"].ToString());

            OrderInfo order = new OrderInfo();

            order.ImageId = (reader["ImageID"].ToString() == "") ? 0 : long.Parse(reader["ImageID"].ToString());
            order.Status = reader["Status"].ToString();
            order.Email = reader["Submittermail"].ToString();
            order.Name = reader["SubmitterName"].ToString();
            order.Phone = reader["SubmitterPhone"].ToString();
            order.Code = reader["SchoolCode"].ToString();
            order.School = reader["SchoolName"].ToString();
            order.Address = reader["SchoolAddress"].ToString();
            order.KidsName = reader["KidsName"].ToString();
            order.Room = reader["KidsRoom"].ToString();
            order.Cards = qtyCard;
            order.Calcs = qtyCalendars;
            order.Diary = qtyDiaries;
            order.Comment = reader["Comment"].ToString();
            order.Country = reader["Country"].ToString();
            order.PrintedId = reader["PrintedImageId"].ToString();
            order.ReOrder = reader["ReOrderImage"].ToString();
            order.BatchBy = reader["BatchBy"].ToString();
            order.Batch = reader["BatchNo"].ToString();
            order.DiaryCrop = reader["DiaryCrop"].ToString();
            order.AuthCode = reader["AuthCode"].ToString();
            order.BatchDate = (!String.IsNullOrEmpty(reader["BatchDate"].ToString()))
                ? (DateTime?)DateTime.Parse(reader["BatchDate"].ToString())
                : null;

            order.CustomerOrderStatus = reader["CustomerOrderStatus"].ToString();
            order.InvoicedDate = (!String.IsNullOrEmpty(reader["InvoicedDate"].ToString()))
                ? (DateTime?)DateTime.Parse(reader["InvoicedDate"].ToString())
                : null;

            order.Vertical = (reader["Vertical"].ToString() != "0") && bool.Parse(reader["Vertical"].ToString());
            order.ContactName = reader["ContactName"].ToString();
            order.Log = reader["Log"].ToString();
            order.OrderType = reader["OrderType"].ToString();
            order.OrderItemId = GetGuidFromObject(reader["UtOrderItemId"]);
            order.OrderId = GetGuidFromObject(reader["UtOrderId"]);
            order.ReorderItemId = GetGuidFromObject(reader["UtSPReorderItemId"]);
            order.SchoolNameId = GetGuidFromObject(reader["utSchoolNameId"]);
            order.SPOrderId = GetGuidFromObject(reader["utSPOrderId"]);

            return order;
        }

        private Guid GetGuidFromObject(object parameter)
        {
            if (parameter == null || String.IsNullOrEmpty(parameter.ToString()))
            {
                return Guid.Empty;
            }

            Guid result;
            try
            {

                result = Guid.Parse(parameter.ToString());
            }
            catch (Exception)
            {
                result = Guid.Empty;
            }

            return result;
        }

        private string GetOrderInfoQuery
        {
            get
            {
                string query = "";
                query = "SELECT       ";
                query += " 0 [ID],   ";
                query += " 'NZ' [Country],   ";
                query += " convert(bigint,convert(nvarchar,o.OrderNumber)+ ";
                query += "         convert(nvarchar,ROW_NUMBER() OVER(PARTITION BY o.OrderNumber ORDER BY oi.Id))) AS [ImageID],  ";
                query += " case when o.SchoolCode is null then   ";
                query += "         case when sn.Code is null then 0 else sn.Code end  else   ";
                query += "         o.SchoolCode end [SchoolCode],   ";
                query += " case when o.OrganisationName is null then   ";
                query += "         case when sn.Name is null then '' else sn.Name end  else   ";
                query += "         o.OrganisationName end [SchoolName],   ";
                query += " isnull(o.DeliveryAddress,'') [SchoolAddress],   ";
                query += " isnull(o.StudentName,'') [KidsName],   ";
                query += " isnull(o.StudentClass,'') [KidsRoom],   ";
                query += " o.Name [SubmitterName],   ";
                query += " o.Email [Submittermail],   ";
                query += " '('+ o.PhoneArea +') '+ o.Phone [SubmitterPhone],   ";
                query += " case when oi.ItemType = 'Cards' then oi.Quantity else 0 end [QtyCard],   ";
                query += " case when oi.ItemType = 'Calendars' then oi.Quantity else 0 end [QtyCalendar],   ";
                query += " case when oi.ItemType = 'Diaries' then oi.Quantity else 0 end [QtyDiary],   ";
                query += " case when (case when o.SchoolCode is null then   ";
                query += "          case when sn.Code is null then 0 else sn.Code end  else   ";
                query += "          o.SchoolCode end = 0 or case when o.SchoolCode is null then   ";
                query += "          case when sn.Code is null then 0 else sn.Code end  else   ";
                query += "          o.SchoolCode end = '') then '#No Code' else 'Good' end [Status],   ";
                query += " '' [BatchBy],   ";
                query += " NULL [BatchNo],   ";
                query += " NULL [BatchDate],   ";
                query += " '' [CustomerOrderStatus],   ";
                query += " '' [PrintedImageId],   ";
                query += " NULL [InvoicedDate],   ";
                query += " isnull(od.PaymentGatewayReference,'') [AuthCode],   ";
                query += " isnull(ro.ImageNumber,'') [ReOrderImage],   ";
                query += " 0 [Vertical],   ";
                query += " '' [DiaryCrop],   ";
                query += " '' [ContactName],   ";
                query += " '' [Comment],   ";
                query += " '' [Log],   ";
                query += " oi.ItemType as [OrderType], ";
                query += " oi.Id [UtOrderItemId], ";
                query += " od.Id [UtOrderId], ";
                query += " ro.Id [UtSPReorderItemId], ";
                query += " sn.Id [utSchoolNameId]	  , ";
                query += "o.Id [utSPOrderId] ";
                query += " FROM utSPOrder o  (nolock) ";
                query += "         INNER JOIN utOrderItem oi (nolock) ON o.Id=oi.OrderId   ";
                query += "         LEFT JOIN utOrder od (nolock) ON oi.OrderId = od.Id   ";
                query += "         LEFT JOIN utSPReorderItems ro (nolock) ON o.Id = ro.OrderID   ";
                query += "         LEFT JOIN utSchoolNames sn (nolock) ON o.OrganisationName = sn.Name  ";
                query += " WHERE o.Finalised=1    AND {0} ";
                query += " GROUP BY oi.Id, o.OrderNumber , oi.ItemType, o.SchoolCode, sn.Code,  ";
                query += "          o.OrganisationName, sn.Name, o.DeliveryAddress,   ";
                query += "          o.StudentName , o.StudentClass, o.Name, o.Email,   ";
                query += "          o.PhoneArea, o.Phone, od.PaymentGatewayReference,   ";
                query += "          ro.ImageNumber,oi.Quantity, od.Id, ro.Id, sn.Id, o.Id ";
                return query;
            }
        }



        public IEnumerable<OrderChangeLog> GetOrderChangeHistory()
        {
            using(var context = new AbacusEntities())
            {
                return context.OrdersChangeLogs.Select(c => new OrderChangeLog()
                                                                {
                                                                    Id = c.Id,
                                                                    ImageId = c.ImageId,
                                                                    NewQuantity = c.NewQuantity,
                                                                    NewRoom = c.NewRoom,
                                                                    NewSchoolCode = c.NewSchoolCode,
                                                                    OldQuantity = c.OldQuantity,
                                                                    OldRoom = c.OldRoom,
                                                                    OldSchoolCode = c.OldSchoolCode,
                                                                    OrderType = c.OrderType
                                                                }).ToList();
            }
        }

    }
}
