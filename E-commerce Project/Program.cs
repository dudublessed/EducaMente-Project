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

            // Cria uma nova conexão com o MySql usando a string de conexão fornecida.
            // Em seguida, tenta abrir a conexão e executa o bloco de código protegido por try-catch para lidar com possíveis falhas na conexão.
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    Wait(1);
                    Console.WriteLine("Connection stablished!");
                    Wait(1);

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

                        // Cria uma instância da classe Random para gerar números aleatórios e define um saldo inicial aleatório para o usuário.
                        // O valor gerado é um inteiro entre 300 e 4000 (inclusive 300, exclusivo 4000), e é convertido para decimal para representar o saldo do usuário.
                        Random rand = new Random();
                        int randomValue = rand.Next(300, 4000);
                        decimal choosenBalance = (decimal)randomValue;

                        // Solicita ao usuário se ele já possui uma conta e converte a resposta para minúsculas para facilitar a comparação.
                        string signAnswer = AskForInput("Você já possui uma conta? (Sim) ou (Não)", true).ToLower();

                        Console.Clear();

                        // Usuário não cadastrado. Processo de registro de um novo usuário abaixo.
                        if (signAnswer == "não")
                        {
                            // (RegisterNewUser()) - Chama tal método da classe (Program.cs) que executa todas as funções em relação ao cadastro de usuário.
                            // Após o término do cadastro, o loop é reiniciado (continue) para que possa ser efetuado o login, isto é, se desejado.
                            RegisterNewUser(connection, choosenBalance);
                            continue;
                        }

                        // Usuário já cadastrado. Processo de login do usuário abaixo.
                        if (signAnswer == "sim")
                        {
                            // (UserLogin() - Chama tal método da classe (Program.cs) que executa todas as funções em relação ao login do usuário.
                            UserLogin(connection);
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

                // Recebe uma exceção específica de MySqlException caso ocorra um erro de conexão com o banco de dados.
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Connection failed: {ex.Message}");
                }

                // Recebe qualquer outra exceção não prevista anteriormente.
                // Utilizado como um catch-all para erros inesperados que não sejam especificamente tratados pelos outros blocos catch.
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }

                // O bloco finally é executado após o bloco (try-catch), garantindo que a conexão com o banco de dados seja fechada independentemente de ocorrer uma exceção ou não.
                finally
                {
                    connection.Close();
                    Console.WriteLine("Conexão encerrada.");
                }
            }

        }

        // Método para exibir uma mensagem no console, opcionalmente (Write) ou (WriteLine).
        static void Write(string prompt, bool useWriteLine = false)
        {
            if (useWriteLine)
            {
                Console.Write(prompt);
            }
            else
            {
                Console.WriteLine(prompt);
            }
        }

        // Método que solicita uma entrada do usuário com uma mensagem, opcionalmente usando (Write) ou (WriteLine).
        static string AskForInput(string prompt, bool useWriteLine = false)
        {
            if (useWriteLine)
            {
                Console.WriteLine(prompt);
            }
            else
            {
                Console.Write(prompt);
            }

            return Console.ReadLine();
        }

        // Função para pausar a execução do código por um determinado número de segundos.
        static void Wait(int waitTime)
        {
            // Recebe o tempo (waitTime) em segundos para (definedWaitSeconds).
            TimeSpan definedWaitSeconds = TimeSpan.FromSeconds(waitTime);

            // Transforma o tempo em segundos (definedWaitSeconds) para milissegundos (milliseconds).
            int milliseconds = (int)definedWaitSeconds.TotalMilliseconds;

            // Executa o bloqueio de código.
            Thread.Sleep(milliseconds);
        }

        static void RegisterNewUser(MySqlConnection connection, decimal choosenBalance)
        {
            string hashedPass;
            string userName;
            string password;
            string confirmPassword;

            while (true)
            {
                Console.WriteLine("____________________");
                Console.WriteLine();

                userName = AskForInput("Nome de Usuário: ", false);
                password = AskForInput("Senha: ", false);
                confirmPassword = AskForInput("Confirme a Senha: ", false);

                Console.WriteLine();
                Console.WriteLine("____________________");
                Console.WriteLine();

                // Verifica se as senhas digitadas são diferentes. Se forem, reinicia o processo de registro do usuário.
                if (password != confirmPassword)
                {
                    Console.WriteLine("As senhas não coincidem entre si. Tente novamente.");
                    Wait(2);
                    Console.Clear();
                    continue;
                }

                // Senhas coincidem. Continuando o processo de registro do usuário.
                // Resgata o valor atribuído a (password) e criptografa usando a biblioteca BCrypt.
                hashedPass = BCrypt.Net.BCrypt.HashPassword(password);

                // Criando uma instância da classe (User.cs) com os dados fornecidos (userName, hashedPass e choosenBalance) para registrar um novo usuário.
                User newUser = new User(userName, hashedPass, choosenBalance);

                // Registrando o usuário no banco de dados
                if (newUser.RegisterUser(connection))
                {
                    Console.WriteLine("Usuário registrado com sucesso!");
                    Wait(2);
                    Console.Clear();
                    return;
                }

                // Nome de usuário já cadastrado no banco de dados. Registro mal sucedido.
                Console.WriteLine("Nome de usuário já cadastrado. Por favor, tente novamente.");
                Wait(2);
                Console.Clear();

            }

        }

        
        static void UserLogin(MySqlConnection connection)
        {
            while (true)
            {
                Console.WriteLine("____________________");
                Console.WriteLine();

                string loginUsername = AskForInput("Nome de usuário", false);
                string loginPassword = AskForInput("Senha: ", false);

                Console.WriteLine();
                Console.WriteLine("____________________");
                Console.WriteLine();

                // Criando uma instância da classe User com o nome de usuário e a senha fornecidos para autenticação.
                User loginUser = new User(loginUsername, loginPassword);

                // Verifica se o usuário existe no banco de dados.
                // (UserExists()) - Chama tal método da classe (User.cs) a qual verifica se o usuário está ou não cadastrado no banco de dados.
                bool userExists = loginUser.UserExists(connection);
                if (!userExists)
                {
                    Console.Clear();
                    Console.WriteLine("Usuário inexistente. Por favor, tente novamente.");
                    Wait(2);
                    Console.Clear();
                    continue;
                }

                // (GetPasswordHash()) - Chama tal método da classe (User.cs) que recupera a senha criptografada (PasswordHash) do banco de dados com base no nome de usuário (loginUsername) fornecido.
                string storedHash = loginUser.GetPasswordHash(connection);


                bool passwordMatches = BCrypt.Net.BCrypt.Verify(loginPassword, storedHash);

                // Senhas criptografadas não coincidem entre si. Login mal sucedido.
                if (!passwordMatches)
                {
                    Console.WriteLine("Senha incorreta. Tente novamente.");
                    Wait(2);
                    Console.Clear();
                    continue;
                }

                // Senhas criptografadas coincidem entre si. Login bem-sucedido!
                Console.Clear();
                Console.WriteLine("Login bem-sucedido!");
                Wait(2);
                Console.Clear();

                // (GetUserInfo()) - Chama tal método da classe (User.cs) que recebe as informações (UserName, Balance) do banco de dados de acordo com a instância de login gerada (loginUser).
                User authenticatedUser = loginUser.GetUserInfo(connection);
                Console.WriteLine($"Nome de Usuário: {authenticatedUser.UserName}");
                Console.WriteLine($"Saldo: {authenticatedUser.Balance}");
                break;
            }
        }

    }
}