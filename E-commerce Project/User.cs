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

        // Construtor vazio.
        public User()
        {

        }

        // Construtor com parâmetros.
        public User(string username, string passwordHash, decimal balance = 0)
        {
            UserName = username;
            PasswordHash = passwordHash;
            Balance = balance;
        }

        // (UserExists()) - Método booleano que verifica no banco de dados se o usuário já está cadastrado com base no nome de usuário (Username) fornecido.
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
            // Verifica se o usuário já existe, isto é, se o método booleano UserExists() é True.
            if (UserExists(connection))
            {
                // Usuário já existe no banco de dados. Retornando False para informar que não é possível registrar tal usuário.
                return false;
            }

            // Usuário não existe no banco de dados. Os valores inseridos pelo usuário são registrados no banco de dados.
            string registerUserQuery = "INSERT INTO users (Username, PasswordHash, Balance) VALUES (@Username, @PasswordHash, @Balance)";
            using (MySqlCommand cmd = new MySqlCommand(registerUserQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Username", UserName);
                cmd.Parameters.AddWithValue("@PasswordHash", PasswordHash);
                cmd.Parameters.AddWithValue("@Balance", Balance);
                cmd.ExecuteNonQuery();

                // Login bem-sucedido. Retorna o valor booleano True para que dê sequência ao código em (Program.cs).
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
            string getDataQuery = "SELECT UserId, Username, Balance FROM users WHERE Username = @Username";
            using (MySqlCommand cmd = new MySqlCommand(getDataQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Username", UserName);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User user = new User();
                        user.UserId = Convert.ToInt32(reader["UserId"]);
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

        public void CartPurchase(MySqlConnection connection, decimal userBalance, int actualUserId)
        {
            string cartPurchaseQuery = "UPDATE users SET Balance = @Balance WHERE UserId = @UserId";

            using(MySqlCommand cmd = new MySqlCommand(cartPurchaseQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Balance", userBalance);
                cmd.Parameters.AddWithValue("UserId", actualUserId);

                cmd.ExecuteNonQuery();
            }
        }


    }
}
