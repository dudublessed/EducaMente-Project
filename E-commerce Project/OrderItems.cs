using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce_Project
{
    internal class OrderItems
    {
        public int OrderItemsId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; } 


        public void InsertOrderItems (MySqlConnection connection)
        {
            string insertOrderItemQuery = "INSERT INTO orderitems (Quantity, Price, OrderId, ProductId) VALUES (@Quantity, @Price, @OrderId, @ProductId)";

            using (MySqlCommand cmd = new MySqlCommand(insertOrderItemQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Quantity", Quantity);
                cmd.Parameters.AddWithValue("@Price", Price);
                cmd.Parameters.AddWithValue("@OrderId", OrderId);
                cmd.Parameters.AddWithValue("@ProductId", ProductId);

                cmd.ExecuteNonQuery();
            }
        }

        public List<OrderItems> GetOrderItems (MySqlConnection connection, int orderId)
        {
            string getOrderItemQuery = "SELECT Quantity, Price, ProductId FROM OrderItems WHERE OrderId = @OrderId";
            List<OrderItems> orderItems = new List<OrderItems>();

            using (MySqlCommand cmd = new MySqlCommand(getOrderItemQuery, connection))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        OrderItems item = new OrderItems
                        {
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Price = Convert.ToDecimal(reader["Price"]),
                            ProductId = Convert.ToInt32(reader["ProductId"])
                        };
                        orderItems.Add(item);
                    }
                }
            }
            return orderItems;
        }
    }
}
