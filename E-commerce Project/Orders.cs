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


        public void CreateOrder(MySqlConnection connection)
        {
            string createOrderQuery = "INSERT INTO orders (OrderDate, TotalAmount, OrderStatus, UserId) VALUES (@OrderDate, @TotalAmount, @OrderStatus, @UserId)";

            using (MySqlCommand cmd = new MySqlCommand(createOrderQuery, connection))
            {
                cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                cmd.Parameters.AddWithValue("@OrderStatus", OrderStatus.ToString());
                cmd.Parameters.AddWithValue("@UserId", UserId);

                cmd.ExecuteNonQuery();
                OrderId = (int)cmd.LastInsertedId;
            }
        }
    }
}
