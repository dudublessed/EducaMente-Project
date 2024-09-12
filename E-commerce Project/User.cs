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


        public User()
        {

        }

        public User(string username, string passwordHash, decimal balance = 0)
        {
            UserName = username;
            PasswordHash = passwordHash;
            Balance = balance;
        }


        public bool UserExists(MySqlConnection connection)
        {
            string findUserQuery = "SELECT COUNT(*) FROM users WHERE Username = @Username";

            using (MySqlCommand cmd = new MySqlCommand(findUserQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Username", UserName);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }


        public bool RegisterUser(MySqlConnection connection)
        {
            if (UserExists(connection))
            {
                return false;
            }

            string registerUserQuery = "INSERT INTO users (Username, PasswordHash, Balance) VALUES (@Username, @PasswordHash, @Balance)";
            using (MySqlCommand cmd = new MySqlCommand(registerUserQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Username", UserName);
                cmd.Parameters.AddWithValue("@PasswordHash", PasswordHash);
                cmd.Parameters.AddWithValue("@Balance", Balance);
                cmd.ExecuteNonQuery();
                return true;
            }
        }


        public string GetPasswordHash(MySqlConnection connection)
        {
            string passQuery = "SELECT PasswordHash FROM users WHERE Username = @Username";

            using(MySqlCommand cmd = new MySqlCommand(passQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Username", UserName);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }


        public User GetUserInfo(MySqlConnection connection)
        {
            string getDataQuery = "SELECT Username, Balance FROM users WHERE Username = @Username";
            using (MySqlCommand cmd = new MySqlCommand(getDataQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Username", UserName);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User user = new User();
                        user.UserName = reader["Username"].ToString();
                        user.Balance = Convert.ToDecimal(reader["Balance"]);
                        return user;
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
