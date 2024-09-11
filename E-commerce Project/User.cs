using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using E_commerce_Project;
using System.Security.Cryptography.X509Certificates;

namespace E_commerce_Project
{
    internal class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public decimal Balance { get; set; }


        public bool UserExists(MySqlConnection connection, User newUser)
        {
            string findUserQuery = "SELECT COUNT(*) FROM users WHERE Username = @Username";

            using (MySqlCommand cmd = new MySqlCommand(findUserQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Username", newUser.UserName);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }


        public void InsertUser(MySqlConnection connection, User newUser)
        {
            string insertQuery = "INSERT INTO users (Username, PasswordHash, Balance) Values (@Username, @PasswordHash, @Balance)";


            using(MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Username", newUser.UserName);
                cmd.Parameters.AddWithValue("@PasswordHash", newUser.PasswordHash);
                cmd.Parameters.AddWithValue("@Balance", newUser.Balance);

                cmd.ExecuteNonQuery();

                Console.Clear();

                Console.WriteLine("Usuário registrado com sucesso!");
            }
        }

        public string GetPasswordHash(MySqlConnection connection, User loginUser)
        {
            string passQuery = "SELECT PasswordHash FROM users WHERE Username = @Username";

            using(MySqlCommand cmd = new MySqlCommand(passQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Username", loginUser.UserName);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        public User GetUserInfo(MySqlConnection connection, User authenticatedUser)
        {
            string getDataQuery = "SELECT Username, Balance FROM users WHERE Username = @Username";

            using (MySqlCommand cmd = new MySqlCommand(getDataQuery, connection)) 
            {
                cmd.Parameters.AddWithValue("@Username", authenticatedUser.UserName);

                using (MySqlDataReader reader = cmd.ExecuteReader()) 
                { 
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserName = reader["Username"].ToString(),
                            Balance = Convert.ToDecimal(reader["Balance"])
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

    }
}
