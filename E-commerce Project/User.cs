using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using E_commerce_Project;

namespace E_commerce_Project
{
    internal class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public decimal Balance { get; set; }



        public void InsertUser(MySqlConnection connection, User newUser)
        {
            string query = "INSERT INTO users (Username, PasswordHash, Balance) Values (@Username, @PasswordHash, @Balance)";

            using(MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Username", newUser.UserName);
                cmd.Parameters.AddWithValue("@PasswordHash", newUser.PasswordHash);
                cmd.Parameters.AddWithValue("@Balance", newUser.Balance);

                cmd.ExecuteNonQuery();

                Console.Clear();

                Console.WriteLine("Usuário registrado com sucesso!");
            }
        }

    }
}
