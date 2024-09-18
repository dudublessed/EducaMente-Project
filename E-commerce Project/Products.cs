using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce_Project
{
    internal class Products
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }


        public List<Products> GetProducts(MySqlConnection connection)
        {
            string productsQuery = "SELECT Name, Description, Author, Price, Stock FROM products WHERE CategoryId = @CategoryId";

            List<Products> productsList = new List<Products>(); 

            using(MySqlCommand cmd = new MySqlCommand(productsQuery, connection))
            {
                cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {

                        Products product = new Products
                        {
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            Author = reader["Author"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Stock = Convert.ToInt32(reader["Stock"])
                        };
                        productsList.Add(product);
                    }
                }
            }
            return productsList;
        }

        public Products GetProductInfo (MySqlConnection connection, string productName)
        {
            string getProductQuery = "SELECT ProductId, Name, Price, Stock FROM products WHERE Name = @Name";

            using (MySqlCommand cmd = new MySqlCommand(getProductQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Name", productName);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Products
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            Name = reader["Name"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Stock = Convert.ToInt32(reader["Stock"])
                        };
                    }
                    else
                    {
                        throw new Exception("Produto não encontrado. Por favor, tente novamente.");
                    }
                }
            }

        }


        public Products GetProductById(MySqlConnection connection, int productId)
        {
            string getProductQuery = "SELECT ProductId, Name, Price, Stock FROM products WHERE ProductId = @ProductId";

            using (MySqlCommand cmd = new MySqlCommand(getProductQuery, connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Products
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            Name = reader["Name"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Stock = Convert.ToInt32(reader["Stock"])
                        };
                    }
                    else
                    {
                        throw new Exception("Produto não encontrado. Por favor, tente novamente.");
                    }
                }
            }

        }
    }

}
