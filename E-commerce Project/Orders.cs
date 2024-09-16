using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce_Project
{
    internal class Orders
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public enum Status
        {
            Pending,
            Completed,
            Cancelled
        }
        public Status OrderStatus { get; set; }
        public int UserId { get; set; }
    }
}
