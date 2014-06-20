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
        public IQueryable<OrderInfo> GetAllOrders()
        {
            using(AbacusEntities context = new AbacusEntities())
            {
                List<OrderInfo> result = new List<OrderInfo>();
                SqlConnection abacusConnection = new SqlConnection(context.Database.Connection.ConnectionString);
                abacusConnection.Open();
                SqlCommand command = new SqlCommand(GetOrderInfoQuery, abacusConnection);
                SqlDataReader reader = command.ExecuteReader();

                while(reader.Read())
                {
                    OrderInfo currentOrderInfo = GetOrderInfoFromReader(reader);
                    if(currentOrderInfo != null)
                    {
                        result.Add(currentOrderInfo);
                    }
                }

                return result.AsQueryable();
            }
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
            return GetAllOrders().Where(o => o.Calcs == 0 && o.Cards == 0 && o.Diary == 0);
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

        private OrderInfo GetOrderInfoFromReader(SqlDataReader reader)
        {
            decimal qtyCard = (reader["QtyCard"].ToString() == "") ? 0 : decimal.Parse(reader["QtyCard"].ToString());
            decimal qtyCalendars = (reader["QtyCalendar"].ToString() == "") ? 0 : decimal.Parse(reader["QtyCalendar"].ToString());
            decimal qtyDiaries = (reader["QtyDiary"].ToString() == "") ? 0 : decimal.Parse(reader["QtyDiary"].ToString());

            if (qtyCard + +qtyCalendars + qtyDiaries > 0)
            {
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

                return order;
            }

            return null;
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
                query += " '' [Log]   ";
                query += " FROM utSPOrder o  (nolock) ";
                query += "         INNER JOIN utOrderItem oi (nolock) ON o.Id=oi.OrderId   ";
                query += "         LEFT JOIN utOrder od (nolock) ON oi.OrderId = od.Id   ";
                query += "         LEFT JOIN utSPReorderItems ro (nolock) ON o.Id = ro.OrderID   ";
                query += "         LEFT JOIN utSchoolNames sn (nolock) ON o.OrganisationName = sn.Name  ";
                query += " WHERE o.Finalised=1    ";
                query += " GROUP BY oi.Id, o.OrderNumber , oi.ItemType, o.SchoolCode, sn.Code,  ";
                query += "          o.OrganisationName, sn.Name, o.DeliveryAddress,   ";
                query += "          o.StudentName , o.StudentClass, o.Name, o.Email,   ";
                query += "          o.PhoneArea, o.Phone, od.PaymentGatewayReference,   ";
                query += "          ro.ImageNumber,oi.Quantity ";
                return query;
            }
        }
    }
}
