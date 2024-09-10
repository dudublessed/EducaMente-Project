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
                    Thread.Sleep(1000);
                    Console.WriteLine("Connection stablished!");
                    Thread.Sleep(1000);

                    // Continue from there.

                    Random rand = new Random(); 
                    int randomValue = rand.Next(300, 4000);
                    decimal choosenBalance = (decimal)randomValue;

                    Console.Clear();
                    Thread.Sleep(2000);
                    Console.WriteLine("Bem-vindo ao EducaMente!");
                    Console.WriteLine();
                    Thread.Sleep(2000);
                    Console.WriteLine("Este é um projeto e-commerce da EducaMente que fornece livros diversos tipos com bons preços para os bons leitores!");
                    Console.WriteLine();
                    Thread.Sleep(1000);

                    // Novo Usuário 
                    while (true)
                    {
                        string hasAccount = "Você já possui uma conta? (Sim) ou (Não)";
                        Console.WriteLine(hasAccount);
                        string signAnswer = Console.ReadLine();

                        Console.Clear();

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

                            // Dados de registro prontos, enviando as informações do novo usuário para a classe User.cs como newUser

                            User newUser = new User
                            {
                                UserName = answerUserName,
                                PasswordHash = hashedPass,
                                Balance = choosenBalance

                            };

                            // Chamando o método que verifica se já existe outro usuário já existente com o nome inserido

                            if (newUser.UserExists(connection, newUser) == false)
                            {
                                // Chamando o método que insere o novo usuário no banco de dados MySql
                                newUser.InsertUser(connection, newUser);
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Nome de usuário já cadastrado. Por favor, tente novamente.");
                                Console.WriteLine();
                                Thread.Sleep(2000);
                                Console.Clear();
                                continue;
                            }

                        }
                        else if (signAnswer.ToLower() == "sim")
                        {

                        }
                        else
                        {
                            Console.WriteLine("Opção inválida. Por favor, tente novamente.");
                            Thread.Sleep(3000);
                            Console.Clear();
                            continue;

                        }
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