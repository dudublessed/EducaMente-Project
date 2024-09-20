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

        public void CancelPurchase(MySqlConnection connection, int orderId)
        {
            string cancelPurchase = "UPDATE orders SET OrderStatus = @OrderStatus WHERE OrderId = @OrderId";

            using (MySqlCommand cmd = new MySqlCommand(cancelPurchase, connection))
            {
                cmd.Parameters.AddWithValue("@OrderStatus", "Cancelled");
                cmd.Parameters.AddWithValue("@OrderId", orderId);

                cmd.ExecuteNonQuery();
            }
        }

        public void CompletePurchase(MySqlConnection connection, int orderId)
        {
            string completePurchase = "UPDATE orders SET OrderStatus = @OrderStatus WHERE OrderId = @OrderId";

            using(MySqlCommand cmd = new MySqlCommand(completePurchase, connection))
            {
                cmd.Parameters.AddWithValue("@OrderStatus", "Completed");
                cmd.Parameters.AddWithValue("@OrderId", orderId);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateOrderTotalAmount(MySqlConnection connection, decimal totalAmount)
        {
            string updateOrderTotalAmountQuery = "UPDATE orders SET TotalAmount = @TotalAmount WHERE OrderId = @OrderId";

            using(MySqlCommand cmd = new MySqlCommand (updateOrderTotalAmountQuery, connection))
            {
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                cmd.Parameters.AddWithValue("@OrderId", OrderId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
