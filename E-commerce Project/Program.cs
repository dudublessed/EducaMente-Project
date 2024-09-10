using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using E_commerce_Project;


namespace Ecommerce
{
    class Program
    {
        static void Main(string[] args)
        {
    
            string connectionString = "Server=localhost;Database=e_commerce;User ID=root;Password=Iloveduke123!;Pooling=true;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection stablished!");

                    // Continue from there.

                    Random rand = new Random(); 
                    int randomValue = rand.Next(300, 4000);
                    decimal choosenBalance = (decimal)randomValue;

                    Console.Clear();
                    Console.WriteLine("Bem-vindo ao EducaMente!");
                    Console.WriteLine();
                    Console.WriteLine("Este é um projeto e-commerce da EducaMente que fornece livros diversos tipos com bons preços para os bons leitores!");
                    Console.WriteLine();
                    Console.WriteLine("Você já possui uma conta? (Sim) ou (Não)");
                    string signAnswer = Console.ReadLine();

                    Console.Clear();

                    // Novo Usuário 
                    if (signAnswer.ToLower() == "não")
                    {
                        string hashedPass;
                        Console.WriteLine("____________________");
                        Console.WriteLine();
                        Console.WriteLine("Nome de Usuário: ");
                        string answerUserName = Console.ReadLine();
                        while (true)
                        {
                            Console.WriteLine("Senha: ");
                            string firstPass = Console.ReadLine();
                            Console.WriteLine("Confirme a Senha: ");
                            string secondPass = Console.ReadLine();

                            if (firstPass != secondPass)
                            {
                                Console.WriteLine("As senhas não coincidem entre si. Tente novamente.");
                                Console.SetCursorPosition(0, Console.CursorTop - 1);
                                continue;
                            }
                            else if (firstPass == secondPass)
                            {
                                hashedPass = BCrypt.Net.BCrypt.HashPassword(firstPass);
                                break;
                            }

                        }

                        // Processo de enviar as informações a classe usuário

                        User newUser = new User
                        {
                            UserName = answerUserName,
                            PasswordHash = hashedPass,
                            Balance = choosenBalance

                        };

                        newUser.InsertUser(connection, newUser);

                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Connection failed {ex.Message}");
                }

                finally
                {
                    connection.Close();
                    Console.WriteLine("Conexão encerrada.");
                }
            }


        }
    }
}