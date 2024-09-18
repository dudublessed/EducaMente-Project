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
        static int actualUserId;

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
                    Write("Connection stablished!", true);
                    Wait(1);
                    Console.Clear();

                    Wait(2);

                    Write("Bem-vindo ao EducaMente!\n", true);

                    Wait(2);

                    Write("Este é um projeto e-commerce da EducaMente que fornece livros diversos tipos com bons preços para os bons leitores!\n", true);

                    Wait(2);

                    while (true)
                    {

                        // Cria uma instância da classe Random para gerar números aleatórios e define um saldo inicial aleatório para o usuário.
                        // O valor gerado é um inteiro entre 300 e 4000 (inclusive 300, exclusivo 4000), e é convertido para decimal para representar o saldo do usuário.
                        Random rand = new Random();
                        int randomValue = rand.Next(300, 4000);
                        decimal choosenBalance = (decimal)randomValue;

                        // Solicita ao usuário se ele já possui uma conta e converte a resposta para minúsculas para facilitar a comparação.
                        string signAnswer = AskForInput("Você já possui uma conta? (Sim) ou (Não)", true).ToLower().Trim();

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
                            Wait(1);
                        }

                        // Usuário não selecionou nenhuma das opções fornecidas, a saber, (Sim) ou (Não). Reiniciando tentativa.
                        else
                        {
                            Write("Opção inválida. Por favor, tente novamente.", true);
                            Wait(3);
                            Console.Clear();
                            continue;

                        }

                        break;
                    }

                    // Login bem-sucedido. Prosseguir aqui!
                    string selectedCategoryName;
                    string selectedProductName;

                    int categoryId = 0;
                    int orderId = 0;
                    int productId = 0;
                    int productQuantity = 0;

                    decimal productPrice = 0m;
                    decimal totalAmount = 0m;

                    OrderItems orderItemsHandler = new OrderItems();

                    Orders newOrder = new Orders
                    {
                        TotalAmount = totalAmount,  
                        OrderStatus = Orders.Status.Pending,
                        UserId = actualUserId
                    };

                    newOrder.CreateOrder(connection);

                    orderId = newOrder.OrderId;

                    while (true)
                    {
                        Write("Selecione uma das categorias de livros abaixo para explorar:", true);

                        List<Category> categories = new Category().GetCategories(connection);

                        Write("____________________\n", true);

                        foreach (var category in categories)
                        {
                            Write($"\n{category.Name}", true);
                            Write($"{category.Description}.\n", true);
                        }

                        Write("____________________\n", true);

                        selectedCategoryName = AskForInput("Digite o nome da categoria desejada: ", false).Trim();

                        try
                        {
                            categoryId = new Category().GetCategoryIdByName(connection, selectedCategoryName);
                            
                        }
                        catch (Exception ex)
                        {
                            Console.Clear();
                            Write($"{ex.Message}\n");
                            Wait(3);
                            Console.Clear();
                            continue;
                        }
                    

                        List<Products> products = new Products() { CategoryId = categoryId }.GetProducts(connection);

                         Write("____________________\n", true);

                        foreach (var product in products)
                        {
                            Write($"\nNome: {product.Name}", true);
                            Write($"Descrição: {product.Description}.", true);
                            Write($"Autor: {product.Author}", true);
                            Write($"Preço: {product.Price:C}", true);
                            Write($"Estoque: {product.Stock}\n", true);
                        }

                        Write("____________________\n", true);


                        selectedProductName = AskForInput("Qual livro você gostaria de adquirir dessa categoria? ", false);

                        productQuantity = Convert.ToInt32(AskForInput("\nQuantidade: ", false).Trim());

                        try
                        {
                            Products product = new Products().GetProductInfo(connection, selectedProductName);

                            productId = product.ProductId;
                            productPrice = product.Price;

                            var newOrderItem = new OrderItems
                            {
                                OrderId = orderId,
                                ProductId = productId,
                                Quantity = productQuantity,
                                Price = productPrice
                            };
                            newOrderItem.InsertOrderItems(connection);

                            totalAmount = 0m;

                            List<OrderItems> itemsInOrder = new OrderItems().GetOrderItems(connection, orderId);

                            Write("\n____________________\n", true);
                            Write($"Itens do Pedido [{orderId}]: \n", true);
                            foreach(var item in itemsInOrder)
                            {
                                Products productDetails = new Products().GetProductById(connection, item.ProductId);

                                Write($"Produto: {productDetails.Name}", true);
                                Write($"Quantidade: {item.Quantity}", true);
                                Write($"Preço unitário: {item.Price:C}\n", true);

                                totalAmount += item.Quantity * item.Price;

                            }
                            newOrder.UpdateOrderTotalAmount(connection, totalAmount);

                            Write($"Subtotal: {totalAmount:C}\n", true);
                            Write("____________________\n", true);

                            string addItemAnswer;
                            string editOrderAnswer;
                            string removeAnotherItemAnswer;
                            string itemToRemove;


                            while (true)
                            {
                                addItemAnswer = AskForInput($"Deseja adicionar outro item? Digite (Sim) ou (Não)", true).ToLower().Trim();

                                if (addItemAnswer == "sim" || addItemAnswer == "não")
                                {
                                    break;  
                                }

                                Write("\nOpção inválida. Por favor, tente novamente.", true);
                                Wait(3);
                                continue;

                            }


                            if (addItemAnswer == "sim")
                            {
                                Console.Clear();
                                Wait(1);
                                continue;
                            }


                            while(true)
                            {
                                 editOrderAnswer = AskForInput($"\nDeseja remover algum item do seu pedido? Digite (Sim) ou (Não)", true).ToLower().Trim();

                                 if (editOrderAnswer == "sim" || editOrderAnswer == "não")
                                 {
                                        break; 
                                 }

                                 Write("\nOpção inválida. Por favor, tente novamente.", true);
                                 Wait(3);
                                 continue;
                            }

                            if (editOrderAnswer == "sim")
                            {
                                while (true)
                                {
                                    itemToRemove = AskForInput($"\nDigite o nome do item que deseja remover: ", false);

                                    product.GetProductInfo(connection, itemToRemove);

                                    productId = product.ProductId;
                                    productPrice = product.Price;

                                    bool itemExists = orderItemsHandler.ItemExists(connection, productId, orderId);


                                    if (!itemExists)
                                    {
                                        Write($"\nEsse item não está em seu carrinho. Tente novamente.", true);
                                        Wait(3);
                                        continue;
                                    }

                                    orderItemsHandler.RemoveOrderItems(connection, productId, orderId);

                                    itemsInOrder.RemoveAll(item => item.ProductId == productId);

                                    Write($"\n{itemToRemove} foi removido com sucesso!\n", true);

                                    totalAmount -= productPrice;
                                    newOrder.UpdateOrderTotalAmount(connection, totalAmount);

                                    itemsInOrder = orderItemsHandler.GetOrderItems(connection, newOrder.OrderId);

                                    Write("\n____________________\n", true);
                                    Write($"Itens do Pedido: [{orderId}]: \n", true);
                                    foreach (var item in itemsInOrder)
                                    {
                                        Products productDetails = new Products().GetProductById(connection, item.ProductId);

                                        Write($"Produto: {productDetails.Name}", true);
                                        Write($"Quantidade: {item.Quantity}", true);
                                        Write($"Preço unitário: {item.Price:C}\n", true);


                                    }
                                    Write($"Subtotal: {totalAmount:C}\n", true);
                                    Write("____________________\n", true);

                                    while (true)
                                    {
                                        removeAnotherItemAnswer = AskForInput($"\nDeseja remover outro item? Digite (Sim) ou (Não)", true).ToLower().Trim();

                                        if (removeAnotherItemAnswer == "sim" || removeAnotherItemAnswer == "não")
                                        {
                                            break;
                                        }
                                        else if (removeAnotherItemAnswer != "sim")
                                        {
                                            Write("\nOpção inválida. Por favor, tente novamente.", true);
                                            Wait(3);
                                            continue;
                                        }
                                    }

                                    if (removeAnotherItemAnswer == "sim")
                                    {
                                        continue;
                                    }

                                    
                                    break;

                                }
                            }

                            itemsInOrder = orderItemsHandler.GetOrderItems(connection, newOrder.OrderId);

                            Write("\n____________________\n", true);
                            Write($"Carrinho: [{orderId}]: \n", true);
                            foreach (var item in itemsInOrder)
                            {
                                Products productDetails = new Products().GetProductById(connection, item.ProductId);

                                Write($"Produto: {productDetails.Name}", true);
                                Write($"Quantidade: {item.Quantity}", true);
                                Write($"Preço unitário: {item.Price:C}\n", true);


                            }
                            Write($"Subtotal: {totalAmount:C}\n", true);
                            Write("____________________\n", true);


                            // Prosseguir para a compra!

                            break;

                        }
                        catch (Exception ex)
                        {

                            Write($"{ex.Message}\n");
                            Wait(5);
                            Console.Clear();
                            continue;
                        }

                    }

                }

                // Recebe uma exceção específica de MySqlException caso ocorra um erro de conexão com o banco de dados.
                catch (MySqlException ex)
                {
                    Write($"Connection failed: {ex.Message}", true);
                }

                // Recebe qualquer outra exceção não prevista anteriormente.
                // Utilizado como um catch-all para erros inesperados que não sejam especificamente tratados pelos outros blocos catch.
                catch (Exception ex)
                {
                    Write($"Unexpected error: {ex.Message}", true);
                }

                // O bloco finally é executado após o bloco (try-catch), garantindo que a conexão com o banco de dados seja fechada independentemente de ocorrer uma exceção ou não.
                finally
                {
                    connection.Close();
                    Write("Conexão encerrada.", true);
                }
            }

        }

        // Método para exibir uma mensagem no console, opcionalmente (Write) ou (WriteLine).
        static void Write(string prompt, bool useWriteLine = false)
        {
            if (useWriteLine)
            {
                Console.WriteLine(prompt);
            }
            else
            {
                Console.Write(prompt);
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
                Write("____________________\n", true);

                userName = AskForInput("Nome de Usuário: ", false).Trim();
                password = AskForInput("Senha: ", false).Trim();
                confirmPassword = AskForInput("Confirme a Senha: ", false).Trim();

                Write("\n____________________\n", true);


                // Verifica se as senhas digitadas são diferentes. Se forem, reinicia o processo de registro do usuário.
                if (password != confirmPassword)
                {
                    Write("As senhas não coincidem entre si. Tente novamente.", true);
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
                    Write("Usuário registrado com sucesso!", true);
                    Wait(2);
                    Console.Clear();
                    return;
                }

                // Nome de usuário já cadastrado no banco de dados. Registro mal sucedido.
                Write("Nome de usuário já cadastrado. Por favor, tente novamente.", true);
                Wait(2);
                Console.Clear();

            }

        }

        
        static void UserLogin(MySqlConnection connection)
        {
            while (true)
            {
                Write("____________________\n", true);

                string loginUsername = AskForInput("Nome de usuário: ", false).Trim();
                string loginPassword = AskForInput("Senha: ", false).Trim();

                Write("\n____________________\n", true);

                // Criando uma instância da classe User com o nome de usuário e a senha fornecidos para autenticação.
                User loginUser = new User(loginUsername, loginPassword);

                // Verifica se o usuário existe no banco de dados.
                // (UserExists()) - Chama tal método da classe (User.cs) a qual verifica se o usuário está ou não cadastrado no banco de dados.
                bool userExists = loginUser.UserExists(connection);
                if (!userExists)
                {
                    Console.Clear();
                    Write("Usuário inexistente. Por favor, tente novamente.", true);
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
                    Write("Senha incorreta. Tente novamente.", true);
                    Wait(2);
                    Console.Clear();
                    continue;
                }

                // (GetUserInfo()) - Chama tal método da classe (User.cs) que recebe as informações (UserName, Balance) do banco de dados de acordo com a instância de login gerada (loginUser).
                User authenticatedUser = loginUser.GetUserInfo(connection);
                actualUserId = authenticatedUser.UserId;

                // Senhas criptografadas coincidem entre si. Login bem-sucedido!
                Console.Clear();
                Write($"Login bem-sucedido! Bem-vindo(a), {authenticatedUser.UserName}!\n", true);
                Wait(1);
                Write($"Seu saldo disponível é: {authenticatedUser.Balance:C}.\n", true);
                break;
            }
        }

    }
}