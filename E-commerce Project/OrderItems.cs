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

        public bool ItemExists (MySqlConnection connection, int productId, int orderId)
        {
            string searchQuery = "SELECT EXISTS (SELECT 1 FROM orderitems WHERE ProductId = @ProductId AND OrderId = @OrderId)";

            using (MySqlCommand cmd = new MySqlCommand(searchQuery, connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                return Convert.ToBoolean(cmd.ExecuteScalar());
            }

        }

        public void RemoveOrderItems (MySqlConnection connection, int productId, int orderId)
        {
            string removeOrderItemQuery = "DELETE FROM orderitems WHERE ProductId = @ProductId AND OrderId = @OrderId";

            using(MySqlCommand cmd = new MySqlCommand(removeOrderItemQuery, connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@OrderId", orderId);

                cmd.ExecuteNonQuery();
            }

        }

        public int GetOrderId (MySqlConnection connection, int productId)
        {
            string getOrderIdQuery = "SELECT OrderId FROM orderitems WHERE ProductId = @ProductId";

            using(MySqlCommand cmd = new MySqlCommand(getOrderIdQuery, connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);

                int orderId = Convert.ToInt32(cmd.ExecuteScalar());

                return orderId;
            }
        }

        public List<OrderItems> GetOrderItems (MySqlConnection connection, int orderId)
        {
            string getOrderItemQuery = "SELECT Quantity, Price, ProductId FROM orderitems WHERE OrderId = @OrderId";
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
