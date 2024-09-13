using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce_Project
{
    internal class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public List<Category> GetCategories(MySqlConnection connection)
        {
            string categoriesShowQuery = "SELECT Name, Description FROM categories";

            List<Category> categoriesList = new List<Category>();

            using(MySqlCommand cmd = new MySqlCommand (categoriesShowQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Name", Name);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Category category = new Category
                        {
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString()
                        };
                        categoriesList.Add(category);
                    }
                }
            }

            return categoriesList;
        }

        public int GetCategoryIdByName(MySqlConnection connection, string categoryName)
        {
            string categoriesIdQuery = "SELECT CategoryId FROM categories WHERE Name = @Name";

            using(MySqlCommand cmd = new MySqlCommand (categoriesIdQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Name", categoryName);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }

                throw new Exception("Categoria não encontrada. Por favor, tente novamente.");

            }
        }
    }
}
