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
    
            // String de conexão para com o banco de dados MySql. 
            string connectionString = "Server=localhost;Database=e_commerce;User ID=root;Password=Iloveduke123!;Pooling=true;";

            // Cria uma nova conexão com o MySql de acordo com as informações apresentadas na string anterior.
            // Em seguida tenta um método (try-catch) para executar a porção do código correspondente a um sucesso ou fracasso na conexão para com o banco de dados.
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Wait(1);
                    Console.WriteLine("Connection stablished!");
                    Wait(1);

                    Random rand = new Random(); 
                    int randomValue = rand.Next(300, 4000);
                    decimal choosenBalance = (decimal)randomValue;

                    Console.Clear();
                    Wait(2);
                    Console.WriteLine("Bem-vindo ao EducaMente!");
                    Console.WriteLine();
                    Wait(2);
                    Console.WriteLine("Este é um projeto e-commerce da EducaMente que fornece livros diversos tipos com bons preços para os bons leitores!");
                    Console.WriteLine();
                    Wait(2);


                    while (true)
                    {
                        string hasAccount = "Você já possui uma conta? (Sim) ou (Não)";
                        Console.WriteLine(hasAccount);
                        string signAnswer = Console.ReadLine();

                        Console.Clear();

                        // Usuário não cadastrado. Processo de registro de um novo usuário abaixo.
                        if (signAnswer.ToLower() == "não")
                        {
                            string hashedPass;
                            string answerUserName;
                            string firstPass;
                            string secondPass;

                            while (true)
                            {
                                Console.WriteLine("____________________");
                                Console.WriteLine();
                                Console.Write("Nome de Usuário: ");
                                answerUserName = Console.ReadLine();


                                Console.Write("Senha: ");
                                firstPass = Console.ReadLine();
                                Console.Write("Confirme a Senha: ");
                                secondPass = Console.ReadLine();
                                Console.WriteLine();
                                Console.WriteLine("____________________");
                                Console.WriteLine();

                                // Confere se as senhas digitadas são diferentes. Se sim, ele reinicia o processo de registro do usuário.
                                if (firstPass != secondPass)
                                {
                                    Console.Clear();
                                    Console.WriteLine("As senhas não coincidem entre si. Tente novamente.");
                                    Wait(2);
                                    Console.Clear();
                                    continue;
                                }

                                // Senhas coincidem. Continuando o processo de registro do usuário.
                                // Resgata o valor inserido em (firstPass) e criptografa usando a biblioteca BCrypt.
                                hashedPass = BCrypt.Net.BCrypt.HashPassword(firstPass);
                                break;
                                
                            }

                            // Criando uma instância da classe (User.cs) com os dados fornecidos (answerUserName, hashedPass e choosenBalance) para registrar um novo usuário.
                            User newUser = new User
                            {
                                UserName = answerUserName,
                                PasswordHash = hashedPass,
                                Balance = choosenBalance

                            };

                            // (UserExists()) - Chama tal método da classe (User.cs) a qual verifica se o usuário está ou não cadastrado no banco de dados.
                            // Nome de usuário não cadastrado no banco de dados. Registro bem sucedido!
                            if (newUser.UserExists(connection, newUser) == false)
                            {
                                // (InserUser()) - Chama tal método da classe (User.cs) a qual insere o usuário no banco de dados.
                                newUser.InsertUser(connection, newUser);
                                Wait(2);
                                Console.Clear();
                            }

                            // Nome de usuário já cadastrado no banco de dados. Registro mal sucedido.
                            Console.Clear();
                            Console.WriteLine("Nome de usuário já cadastrado. Por favor, tente novamente.");
                            Console.WriteLine();
                            Wait(2);
                            Console.Clear();
                            continue;       

                        }

                        // Usuário já cadastrado. Processo de login do usuário abaixo.
                        if (signAnswer.ToLower() == "sim")
                        {
                            string loginUsername;
                            string loginPassword;

                            while(true)
                            {
                                Console.WriteLine("____________________");
                                Console.WriteLine();
                                Console.Write("Nome de Usuário: ");
                                loginUsername = Console.ReadLine();

                                Console.Write("Senha: ");
                                loginPassword = Console.ReadLine();

                                Console.WriteLine();
                                Console.WriteLine("____________________");
                                Console.WriteLine();

                                // Criando uma instância da classe (User.cs) com o nome de usuário e a senha fornecidos para autenticação (loginUsername e loginPassword).
                                User loginUser = new User
                                {
                                    UserName = loginUsername,
                                    PasswordHash = loginPassword
                                };

                                // (UserExists()) - Chama tal método da classe (User.cs) a qual verifica se o usuário está ou não cadastrado no banco de dados.
                                // Nome de usuário cadastrado no banco de dados. 
                                if (loginUser.UserExists(connection, loginUser) == true)
                                {
                                    // (GetPasswordHash()) - Chama o método da classe (User.cs) que recupera a senha criptografada (PasswordHash) do banco de dados com base no nome de usuário (loginUsername) fornecido.
                                    string storedHash = loginUser.GetPasswordHash(connection, loginUser);

                                    // Senhas criptografadas coincidem entre si. Login bem sucedido!
                                    if (BCrypt.Net.BCrypt.Verify(loginPassword, storedHash))
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Login bem-sucedido!");
                                        Wait(2);
                                        Console.Clear();

                                        User authenticatedUser = loginUser.GetUserInfo(connection, loginUser);
                                        Console.WriteLine($"Nome de Usuário: {authenticatedUser.UserName}");
                                        Console.WriteLine($"Saldo: {authenticatedUser.Balance}");
                                        break;
                                    }

                                    // Senhas criptografadas não coincidem entre si. Login mal sucedido.
                                    Console.WriteLine("Senha incorreta. Tente novamente.");
                                    Wait(2);
                                    Console.Clear();
                                    continue;

                                }

                                // Nome de usuário não está cadastrado no banco de dados. Login mal sucedido.
                                Console.Clear();
                                Console.WriteLine("Usuário inexistente. Por favor, tente novamente.");
                                Wait(2);
                                Console.Clear();
                                continue;
                                
                            }
                        }

                        // Usuário não selecionou nenhuma das opções fornecidas, a saber, (Sim) ou (Não). Reiniciando tentativa.
                        else
                        {
                            Console.WriteLine("Opção inválida. Por favor, tente novamente.");
                            Wait(3);
                            Console.Clear();
                            continue;

                        }

                        break;
                    }

                    // Login bem-sucedido. Prosseguir aqui!
                    Console.WriteLine("Olá!");
                }

                // Recebe um (Exception error) caso tenha sido detectado durante o bloco (try) do (try-catch).
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Connection failed {ex.Message}");
                }


                // Após toda a execução do bloco (try-catch) a conexão com o banco de dados é encerrada, tal como a do programa.
                finally
                {
                    connection.Close();
                    Console.WriteLine("Conexão encerrada.");
                }
            }

        }

        // Função para chamar o Thread.Sleep de forma mais intuitiva e prática.
        static void Wait(int waitTime)
        {
            // Recebe o tempo (waitTime) em segundos para (definedWaitSeconds).
            TimeSpan definedWaitSeconds = TimeSpan.FromSeconds(waitTime);

            // Transforma o tempo em segundos (definedWaitSeconds) para milissegundos (milliseconds).
            int milliseconds = (int)definedWaitSeconds.TotalMilliseconds;

            // Executa o bloqueio de código.
            Thread.Sleep(milliseconds); 
        }
    }
}