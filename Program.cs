using NLog;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DIO.Bank
{
    class Program
    {
        /// <summary>
        /// Lista que armazena as contas de clientes
        /// </summary>
        static List<Cliente> listClientes = new List<Cliente>();
        static List<Operador> listOperadores = new List<Operador>();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //Soluciona problema de diretório variável entre "dotnet run", vscode e visual studio
        static string diretorioBase = AppDomain.CurrentDomain.BaseDirectory; //https://jeremybytes.blogspot.com/2020/02/set-working-directory-in-visual-studio.html

        static string pathListClientes = diretorioBase + "dados\\listClientes.dat";
        static string pathListOperadores = diretorioBase + "dados\\listOperadores.dat";

        static Operador operadorLogado = null;

        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

                AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
                {
                    //https://stackify.com/csharp-exception-handling-best-practices/
                    logger.Warn(eventArgs.Exception.ToString());
                };

                CarregaDados();

                //Cria usuário admin caso não encontre arquivo de operadores
                if (listOperadores.Count == 0)
                {
                    listOperadores.Add(new Operador("admin", "admin", "admin"));
                    //Cria o arquivo que salva os usuários operadores com novo operador Admin
                    ArmazenaDados.SaveList(pathListOperadores, listOperadores);
                }

                //Exibe menus até que usuário escolha S para Sair
                do
                {
                    //todo alterar estrutura de troca de menus pra permitir login e logoff
                    operadorLogado = Operador.ExecutaLoginOperador(listOperadores);
                    if (operadorLogado != null)
                        logger.Info($"Operador [{operadorLogado.Nome}] logado com sucesso!");
                } while (ExibeMenu1() != "S");

                ////mantém console aberto até que pressionem uma tecla:
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Event handler for Process Exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                logger.Info("Saindo...");
                //Salva dados de contas e operadores antes de encerrar:
                ArmazenaDados.SaveList(pathListClientes, listClientes);
                ArmazenaDados.SaveList(pathListOperadores, listOperadores);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            
        }

        /// <summary>
        /// Carrega dados de clientes e operadores
        /// </summary>
        public static void CarregaDados()
        {
            try
            {
                //Carrega lista de clientes/contas
                listClientes = ArmazenaDados.LoadList<Cliente>(pathListClientes);
                if (listClientes.Count == 0)
                {
                    logger.Error("Não foi possível carregar a lista de contas!");
                }
                else
                {
                    logger.Info("Lista de contas carregada com sucesso!");
                }

                //carrega lista de operadores
                listOperadores = ArmazenaDados.LoadList<Operador>(pathListOperadores);
                if (listOperadores.Count == 0)
                {
                    logger.Error("Não foi possível carregar a lista de operadores!");
                }
                else
                {
                    logger.Info("Lista de operadores carregada com sucesso!");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao carregar arquivos: " + ex.Message);
            }

        }

        private static string ExibeMenu1()
        {
            try
            {
                bool isValidOption = false;
                string opcaoUsuario = null;

                while (opcaoUsuario != "S")
                {
                    do
                    {
                        Console.WriteLine();

                        Console.WriteLine("OPERADOR: " + operadorLogado.Nome);
                        Console.WriteLine("Informe a opção desejada:");
                        Console.WriteLine("1- Depositar");
                        Console.WriteLine("2- Sacar");
                        Console.WriteLine("3- Transferir");
                        Console.WriteLine();
                        Console.WriteLine("A- Listar contas");
                        Console.WriteLine("B- Inserir nova conta");
                        Console.WriteLine("C- Excluir conta");
                        Console.WriteLine("D- Alterar senha de conta");
                        Console.WriteLine();
                        Console.WriteLine("E- Inserir Operador");
                        Console.WriteLine("F- Remover Operador");
                        Console.WriteLine("G- Alterar senha de Operador");
                        Console.WriteLine();
                        Console.WriteLine("L- Limpar Tela");
                        Console.WriteLine("S- Sair");

                        Console.WriteLine();
                        opcaoUsuario = Console.ReadLine().ToUpper();

                        switch (opcaoUsuario)
                        {
                            case "1":
                                isValidOption = true;
                                Depositar();
                                break;
                            case "2":
                                isValidOption = true;
                                Sacar();
                                break;
                            case "3":
                                isValidOption = true;
                                Transferir();
                                break;
                            case "A":
                                isValidOption = true;
                                ListarContas();
                                break;
                            case "B":
                                isValidOption = true;
                                InserirConta();
                                break;
                            case "C":
                                isValidOption = true;
                                ExcluirConta();
                                break;
                            case "D":
                                isValidOption = true;
                                AlterarSenhaDeConta();
                                break;
                            case "L":
                                isValidOption = true;
                                Console.Clear();
                                break;
                            case "S":
                                isValidOption = true;
                                break;
                            default:
                                isValidOption = false;
                                Console.WriteLine("Opção inválida!");
                                break;
                        }

                    } while (isValidOption == false);
                }

                return opcaoUsuario;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Solicita dados e altera a senha da conta de cliente
        /// </summary>
        private static void AlterarSenhaDeConta()
        {
            Console.WriteLine("Alterar Senha De Conta");

            Cliente objCliente = Cliente.PedeContaEBuscaCliente(listClientes, pVerboseForAvailability: false);
            if (objCliente == null)
            {
                return;
            }

            Console.Write("Digite a senha atual: ");
            string senhaAntiga = Console.ReadLine();
            //Validação de senha
            if (!Password.CompararSenhas(senhaAntiga, objCliente.Salt, objCliente.Senha))
            {
                Console.WriteLine("Senha incorreta!");
                return;
            }

            Console.Write("Crie a nova senha com 6 dígitos numéricos: ");
            String senhaNova = Console.ReadLine();

            while (!Password.ValidaRegraSenha(senhaNova))
            {
                Console.WriteLine("Por favor, insira uma senha numérica de 6 dígitos.");
                senhaNova = Console.ReadLine();
            }

            if (objCliente.AlteraSenha(senhaAntiga, senhaNova))
            {
                //salva o arquivo incluindo a nova conta
                ArmazenaDados.SaveList(pathListClientes, listClientes);
                logger.Info($"Senha da conta [{objCliente.NumConta} - {objCliente.Nome} ] alterada com sucesso!");
            }

        }//fim AlterarSenhaDeConta()

        /// <summary>
        /// Solicita dados e exclui conta da ListClientes
        /// </summary>
        private static void ExcluirConta()
        {
            Console.WriteLine("Excluir Conta");
            Cliente cliente = null;
            if ((cliente = Cliente.PedeContaEBuscaCliente(pListClientes: listClientes,
                                                            pMsg: "Digite o número da conta a ser excluída: ",
                                                            pVerboseForAvailability: false))
                                                            == null)
            {
                return;
            }

            if (listClientes.Remove(cliente))
            {
                //salva o arquivo incluindo a nova conta
                ArmazenaDados.SaveList(pathListClientes, listClientes);
                logger.Info($"Conta [{cliente.NumConta}] removida com sucesso!");
            }
        }

        /// <summary>
        /// Solicita dados e deposita valor
        /// </summary>
        private static void Depositar()
        {
            Console.WriteLine("Depositar");

            Cliente objCliente = Cliente.PedeContaEBuscaCliente(pListClientes: listClientes, pVerboseForAvailability: false);
            if (objCliente == null)
            {
                return;
            }

            double valorDeposito = EeS.PedeEvalidaDouble("Digite o valor a ser depositado: ");

            objCliente.Depositar(valorDeposito);
            //salva o arquivo incluindo a nova conta
            ArmazenaDados.SaveList(pathListClientes, listClientes);
            logger.Info($"Depósito de {valorDeposito} na conta [{objCliente.NumConta} - {objCliente.Nome}] realizado com sucesso!");
        }

        /// <summary>
        /// Solicita dados e realiza saque
        /// </summary>
        private static void Sacar()
        {
            Console.WriteLine("Sacar");
            Cliente objConta = Cliente.PedeContaEBuscaCliente(listClientes, pVerboseForAvailability: false);
            if (objConta == null)
            {
                return;
            }

            Console.Write("Digite o valor a ser sacado: ");
            double valorSaque = double.Parse(Console.ReadLine());

            Console.Write("Digite a senha: ");
            string senha = Console.ReadLine();

            if (objConta.Sacar(valorSaque, senha))
            {
                //salva o arquivo incluindo a nova conta
                ArmazenaDados.SaveList(pathListClientes, listClientes);
                logger.Info($"Saque de {valorSaque} realizado na conta [{objConta.NumConta} - {objConta.Nome}]");
            }
        }

        /// <summary>
        /// Solicita dados e executa transferência entre contas
        /// </summary>
        private static void Transferir()
        {
            Console.WriteLine("Transferir");
            Cliente clienteOrigem = Cliente.PedeContaEBuscaCliente(listClientes, "Digite o número da conta de origem: ", false);
            if (clienteOrigem == null)
            {
                return;
            }

            Cliente clienteDestino = Cliente.PedeContaEBuscaCliente(listClientes, "Digite o número da conta de destino: ", false);
            if (clienteDestino == null)
            {
                return;
            }

            double valorTransferencia = EeS.PedeEvalidaDouble("Digite o valor a ser transferido: ");

            Console.Write("Digite a senha: ");
            string senha = Console.ReadLine();

            if (clienteOrigem.Transferir(senha, valorTransferencia, clienteDestino))
            {
                //salva o arquivo incluindo a nova conta
                ArmazenaDados.SaveList(pathListClientes, listClientes);
                logger.Info($"Transferência de {valorTransferencia} realizada de conta [{clienteOrigem.NumConta} - {clienteOrigem.Nome}] para a conta [{clienteDestino.NumConta} - {clienteDestino.Nome}]");
            }
        }

        /// <summary>
        /// Solicita dados e insere nova conta de cliente.
        /// </summary>
        private static void InserirConta()
        {
            Console.WriteLine("Inserir nova conta");

            int entradaTipoConta = EeS.PedeEvalidaInteger("Digite 1 para Conta Física ou 2 para Jurídica: ");

            int entradaNumeroConta = EeS.PedeEvalidaInteger("Insira o número desejado para a conta: ");

            bool contaDisponivel = false;
            Cliente cliente = null;
            do
            {
                contaDisponivel = true;
                if ((cliente = Cliente.BuscaConta(pListContas: listClientes,
                                                            entradaNumeroConta,
                                                            pVerboseForAvailability: true))
                                                            != null)
                {
                    contaDisponivel = false;
                    entradaNumeroConta = EeS.PedeEvalidaInteger("Insira outro número para a conta: ");
                }
            } while (contaDisponivel == false);

            Console.Write("Digite o Nome do Cliente: ");
            string entradaNome = Console.ReadLine();

            double entradaSaldo = EeS.PedeEvalidaDouble("Digite o saldo inicial: ");

            double entradaCredito = EeS.PedeEvalidaDouble("Digite o crédito concedido: ");

            Console.Write("Crie a senha com 6 dígitos numéricos: ");
            String entradaSenha = Console.ReadLine();

            while (!Password.ValidaRegraSenha(entradaSenha))
            {
                Console.WriteLine("Por favor, insira uma senha numérica de 6 dígitos.");
                entradaSenha = Console.ReadLine();
            }

            Cliente novaConta = new Cliente(pTipoConta: (TipoConta)entradaTipoConta,
                                        pSaldo: entradaSaldo,
                                        pCredito: entradaCredito,
                                        pNome: entradaNome,
                                        pNumConta: entradaNumeroConta,
                                        pSenha: entradaSenha);

            listClientes.Add(novaConta);
            //salva o arquivo incluindo a nova conta
            ArmazenaDados.SaveList(pathListClientes, listClientes);
            logger.Info("Conta Criada: " + novaConta.ToString());
        }

        /// <summary>
        /// Lista contas em ListClientes
        /// </summary>
        private static void ListarContas()
        {
            Console.WriteLine("Listar contas:");

            if (listClientes.Count == 0)
            {
                Console.WriteLine("Nenhuma conta cadastrada.");
                return;
            }

            for (int i = 0; i < listClientes.Count; i++)
            {
                Cliente conta = listClientes[i];
                Console.WriteLine(conta);
            }
        }

        /// <summary>
        /// Retorna string com linha, caller e arquivo de onde for chamado
        /// </summary>
        /// <returns></returns>
        static string GetCallerLineAndFile(
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string filePath = null)
        {
            string[] fileSplit = filePath.Split("\\");
            string fileName = fileSplit[fileSplit.Length - 1];
            return lineNumber + "|" + caller + "|" + fileName + "|";
        }

    }//fim do programa
}//fim do namespace
