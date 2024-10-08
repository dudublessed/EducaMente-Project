﻿using System;
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
        static decimal userBalance;
        static void Main(string[] args)
        {
            // String de conexão para com o banco de dados MySql. 
            string connectionString = "Server=localhost;Database=e_commerce;User ID=root;Password=Iloveduke123!;Pooling=true;";

            User authenticatedUser = new User();

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
                            UserLogin(connection, authenticatedUser);
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

                    //
                    // Divisão entre processos...
                    //

                    // Login bem-sucedido. Prosseguir aqui!
                    string selectedCategoryName;
                    string selectedProductName;

                    int categoryId = 0;
                    int orderId = 0;
                    int productId = 0;
                    int productQuantity = 0;

                    decimal productPrice = 0m;
                    decimal totalAmount = 0m;

                    // Cria uma instância de OrderItems para manipulação de itens do pedido.
                    OrderItems orderItemsHandler = new OrderItems();

                    // Cria uma nova instância de Orders e define propriedades iniciais.
                    Orders newOrder = new Orders
                    {
                        TotalAmount = totalAmount,  
                        OrderStatus = Orders.Status.Pending, 
                        UserId = actualUserId
                    };

                    // Cria o pedido no banco de dados
                    newOrder.CreateOrder(connection);

                    // Obtém o ID do pedido recém-criado
                    orderId = newOrder.OrderId;

                    // Inicia um loop para permitir que o usuário selecione uma categoria de livros.
                    while (true)
                    {
                        Write("Selecione uma das categorias de livros abaixo para explorar:", true);

                        // Obtém a lista de categorias do banco de dados.
                        List<Category> categories = new Category().GetCategories(connection);

                        // Exibe a lista de categorias
                        Write("____________________\n", true);
                        foreach (var category in categories)
                        {
                            Write($"\n{category.Name}", true);
                            Write($"{category.Description}.\n", true);
                        }
                        Write("____________________\n", true);

                        selectedCategoryName = AskForInput("Digite o nome da categoria desejada: ", false).Trim();

                        // Obtém o ID da categoria com base no nome fornecido.
                        try
                        {
                            categoryId = new Category().GetCategoryIdByName(connection, selectedCategoryName);
                            
                        }

                        // Se ocorrer uma exceção ao obter o ID da categoria, exibe a mensagem de erro e reinicia o loop.
                        catch (Exception ex)
                        {
                            Console.Clear();
                            Write($"{ex.Message}\n");
                            Wait(3);
                            Console.Clear();
                            continue;
                        }

                        // Obtém a lista de produtos na categoria selecionada.
                        List<Products> products = new Products() { CategoryId = categoryId }.GetProducts(connection);

                        // Exibe a lista de produtos
                        Write("____________________\n", true);
                        foreach (var theProduct in products)
                        {
                            Write($"\nNome: {theProduct.Name}", true);
                            Write($"Descrição: {theProduct.Description}.", true);
                            Write($"Autor: {theProduct.Author}", true);
                            Write($"Preço: {theProduct.Price:C}", true);
                            Write($"Estoque: {theProduct.Stock}\n", true);
                        }
                        Write("____________________\n", true);

                        // Solicita ao usuário o nome do livro e a quantidade desejada.
                        selectedProductName = AskForInput("Qual livro você gostaria de adquirir dessa categoria? ", false);
                        productQuantity = Convert.ToInt32(AskForInput("\nQuantidade: ", false).Trim());

                        // Obtém as informações do produto selecionado..
                        Products product = new Products().GetProductInfo(connection, selectedProductName);

                        // Verifica se o estoque é suficiente para a quantidade desejada.
                        // Se o estoque não for suficiente, exibe uma mensagem e reinicia o loop.
                        if (!product.IsStockAvailable(connection, productQuantity)) 
                        {
                            Write($"\nO livro {selectedProductName} está fora de estoque. Por favor, tente novamente.\n");
                            Wait(2);
                            continue;
                        }

                        try
                        {
                            productId = product.ProductId;
                            productPrice = product.Price;

                            // Cria um novo item de pedido com base nas informações fornecidas.
                            var newOrderItem = new OrderItems
                            {
                                OrderId = orderId,
                                ProductId = productId,
                                Quantity = productQuantity,
                                Price = productPrice
                            };

                            // Insere o item de pedido no banco de dados.
                            newOrderItem.InsertOrderItems(connection);

                            // Zera o valor total do pedido
                            totalAmount = 0m;

                            // Obtém a lista de itens no pedido.
                            List<OrderItems> itemsInOrder = new OrderItems().GetOrderItems(connection, orderId);

                            // Exibe os itens do pedido
                            Write("\n____________________\n", true);
                            Write($"Itens do Pedido [{orderId}]: \n", true);
                            foreach(var item in itemsInOrder)
                            {
                                // Obtém as informações detalhadas do produto para cada item do pedido
                                Products productDetails = new Products().GetProductById(connection, item.ProductId);

                                Write($"Produto: {productDetails.Name}", true);
                                Write($"Quantidade: {item.Quantity}", true);
                                Write($"Preço unitário: {item.Price:C}\n", true);

                                // Calcula o subtotal do pedido
                                totalAmount += item.Quantity * item.Price;

                            }

                            // Atualiza o valor total do pedido no banco de dados
                            newOrder.UpdateOrderTotalAmount(connection, totalAmount);

                            // Exibe o subtotal do pedido
                            Write($"Subtotal: {totalAmount:C}\n", true);
                            Write("____________________\n", true);

                            //
                            // Divisão entre processos...
                            //

                            // Processo de adicionar/remover item para com o carrinho.
                            string addItemAnswer;
                            string editOrderAnswer;
                            string removeAnotherItemAnswer;
                            string itemToRemove;

                            // Solicita ao usuário se deseja adicionar outro item ao pedido.
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

                            // Se o usuário deseja adicionar outro item, limpa a tela e reinicia o loop de seleção de itens.
                            if (addItemAnswer == "sim")
                            {
                                Console.Clear();
                                Wait(1);
                                continue;
                            }

                            // Solicita ao usuário se deseja remover algum item do pedido.
                            while (true)
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

                            // Processo de remoção um ou mais itens do pedido.
                            if (editOrderAnswer == "sim")
                            {
                                while (true)
                                {
                                    itemToRemove = AskForInput($"\nDigite o nome do item que deseja remover: ", false);

                                    // Obtém informações do produto baseado no nome fornecido.
                                    product.GetProductInfo(connection, itemToRemove);

                                    productId = product.ProductId;
                                    productPrice = product.Price;

                                    // Verifica se o item está no carrinho do pedido.
                                    bool itemExists = orderItemsHandler.ItemExists(connection, productId, orderId);

                                    // Se o item não está no carrinho, exibe mensagem e reinicia a pergunta.
                                    if (!itemExists)
                                    {
                                        Write($"\nEsse item não está em seu carrinho. Tente novamente.", true);
                                        Wait(3);
                                        continue;
                                    }

                                    // Remove o item do pedido.
                                    orderItemsHandler.RemoveOrderItems(connection, productId, orderId);

                                    // Remove o item da lista de itens no pedido.
                                    itemsInOrder.RemoveAll(item => item.ProductId == productId);

                                    Write($"\n{itemToRemove} foi removido com sucesso!\n", true);

                                    // Atualiza o valor total do pedido subtraindo o preço do item removido.
                                    totalAmount -= productPrice;
                                    newOrder.UpdateOrderTotalAmount(connection, totalAmount);

                                    // Atualiza a lista de itens no pedido.
                                    itemsInOrder = orderItemsHandler.GetOrderItems(connection, newOrder.OrderId);

                                    // Exibe os itens do pedido atualizado e o subtotal.
                                    Write("\n____________________\n", true);
                                    Write($"Itens do Pedido: [{orderId}]: \n", true);
                                    foreach (var item in itemsInOrder)
                                    {
                                        // Obtém as informações detalhadas do produto.
                                        Products productDetails = new Products().GetProductById(connection, item.ProductId);

                                        Write($"Produto: {productDetails.Name}", true);
                                        Write($"Quantidade: {item.Quantity}", true);
                                        Write($"Preço unitário: {item.Price:C}\n", true);


                                    }
                                    Write($"Subtotal: {totalAmount:C}\n", true);
                                    Write("____________________\n", true);

                                    // Pergunta se o usuário deseja remover outro item, caso sim, o loop para remoção é iniciado novamente.
                                    while (true)
                                    {
                                        removeAnotherItemAnswer = AskForInput($"\nDeseja remover outro item? Digite (Sim) ou (Não)", true).ToLower().Trim();

                                        if (removeAnotherItemAnswer == "sim" || removeAnotherItemAnswer == "não")
                                        {
                                            break;
                                        }
                                        else
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

                            // Atualiza a lista final de itens no pedido
                            itemsInOrder = orderItemsHandler.GetOrderItems(connection, newOrder.OrderId);

                            // Exibe os itens finais do pedido e o subtotal
                            Write("\n____________________\n", true);
                            Write($"Carrinho: [{orderId}]: \n", true);
                            foreach (var item in itemsInOrder)
                            {
                                // Obtém as informações detalhadas do produto
                                Products productDetails = new Products().GetProductById(connection, item.ProductId);

                                Write($"Produto: {productDetails.Name}", true);
                                Write($"Quantidade: {item.Quantity}", true);
                                Write($"Preço unitário: {item.Price:C}\n", true);


                            }
                            Write($"Subtotal: {totalAmount:C}\n", true);
                            Write("____________________\n", true);

                            //
                            // Divisão entre processos...
                            //

                            // Processo de compra.

                            string wantToBuyAnswer;

                            // Pergunta se o usuário deseja efetuar a compra.
                            while (true)
                            {
                                wantToBuyAnswer = AskForInput($"Deseja prosseguir com a compra? Digite (Sim) ou (Não).", true).ToLower().Trim();

                                if (wantToBuyAnswer == "sim" || wantToBuyAnswer == "não")
                                {
                                    break;
                                }
                                else
                                {
                                    Write("\nOpção inválida. Por favor, tente novamente.", true);
                                    Wait(3);
                                    continue;
                                }
                            }

                            // Usuário não deseja prosseguir com a compra, cancelando-a e encerrando o programa.
                            if (wantToBuyAnswer == "não")
                            {
                                Write("\nEntendido! Agradecemos sua visita...", true);
                                Wait(2);
                                Write("Encerraremos o programa e cancelaremos a sua compra.", true);
                                newOrder.CancelPurchase(connection, orderId);
                                Wait(2);
                                Environment.Exit(0);
                            }

                            //
                            // Usuário deseja prosseguir com a compra!
                            //

                            // Usuário não tem saldo suficiente para adquirir os produtos do carrinho, cancelando-a e encerrando o programa.
                            if(userBalance < totalAmount)
                            {
                                Write("\nSeu saldo é insuficiente para completar a compra...\n", true);
                                Wait(2);
                                Write("Encerraremos o programa e cancelaremos a sua compra.", true);
                                newOrder.CancelPurchase(connection, orderId);
                                Wait(2);
                                Environment.Exit(0);
                            }

                            // Usuário tem saldo suficiente para adquirir os produtos do carrinho, prosseguindo com a conclusão da compra e encerramento do programa.

                            // Subtrai e atribui o saldo do usuário do valor da compra.
                            userBalance -= totalAmount;

                            // Atualiza o estado do pedido no banco de dados.
                            newOrder.CompletePurchase(connection, orderId);

                            // Atualiza o saldo do usuário no banco de dados.
                            authenticatedUser.CartPurchase(connection, userBalance, actualUserId);

                            // Verifica os itens disponíveis no carrinho baseado na lista.
                            foreach(var item in itemsInOrder)
                            {
                                // Recebe o id do respectivo produto.
                                product.GetProductById(connection, item.ProductId);

                                // Atualiza o valor do estoque do produto no banco de dados.
                                product.UpdateProductStock(connection, item.ProductId, item.Quantity);

                            }

                            // Compra efetuada com sucesso, finalizando programa.
                            Write($"\nCompra efetuada com sucesso!", true);
                            Wait(1);
                            Write($"\nSaldo restante: {userBalance}");
                            Wait(2);
                            Write($"Muito obrigado por utilizar este programa!\n");
                            Wait(3);
                            Environment.Exit(0);

                        }

                        // Recebe uma exceção "geral" e, após exibição da mensagem do respectivo erro, reinicia o código no bloco try.
                        catch (Exception ex)
                        {
                            Console.Clear();
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

        // Função para registrar um novo usuário no banco de dados.
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

        // Função de execução do processo de login do usuário, de acordo com as informações no banco de dados.
        static void UserLogin(MySqlConnection connection, User authenticatedUser)
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
                authenticatedUser = loginUser.GetUserInfo(connection);

                actualUserId = authenticatedUser.UserId;

                userBalance = authenticatedUser.Balance;

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
