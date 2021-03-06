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

        static string pathDataFolder = diretorioBase + "dados\\";
        static string pathListClientesFile = pathDataFolder + "listClientes.dat";
        static string pathListOperadoresFile = pathDataFolder + "listOperadores.dat";

        static Operador operadorLogado = null;

        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

                //AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
                //{
                //    //https://stackify.com/csharp-exception-handling-best-practices/
                //    logger.Warn(eventArgs.Exception.ToString());
                //};

                CarregaDados();

                operadorLogado = Operador.ExecutaLoginOperador(listOperadores);
                if (operadorLogado != null)
                    logger.Trace($"Operador [{operadorLogado.Nome}] logado com sucesso!");

                while (true)
                {
                    if (ExibeMenu1() == "Z")
                    {
                        Console.Clear();
                        operadorLogado = Operador.ExecutaLoginOperador(listOperadores);
                        if (operadorLogado != null)
                            logger.Trace($"Operador [{operadorLogado.Nome}] logado com sucesso!");
                    }
                }

                ////mantém console aberto até que pressionem uma tecla:
                //EeS.ReadConsoleLine();
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
                logger.Trace("Saindo...");
                //Salva dados de contas e operadores antes de encerrar:
                ArmazenaDados.SaveList(pathListClientesFile, listClientes);
                ArmazenaDados.SaveList(pathListOperadoresFile, listOperadores);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Carrega dados de clientes e operadores
        /// </summary>
        private static void CarregaDados()
        {
            try
            {
                ArmazenaDados.VerificaECriaDiretorio(pathDataFolder);

                //Carrega lista de clientes/contas
                listClientes = ArmazenaDados.LoadList<Cliente>(pathListClientesFile);
                if (listClientes != null)
                {
                    logger.Trace($"Lista de clientes carregada: {listClientes.Count} clientes");
                }
                else
                {
                    logger.Error("Não foi possível carregar a lista de contas!");
                }

                //carrega lista de operadores
                listOperadores = ArmazenaDados.LoadList<Operador>(pathListOperadoresFile);
                if (listOperadores == null)
                {
                    logger.Error("Não foi possível carregar a lista de operadores!");

                    //Cria usuário admin caso não encontre arquivo de operadores
                    listOperadores.Add(new Operador("admin", "admin", "Administrador"));
                    //Cria o arquivo que salva os usuários operadores com novo operador Admin
                    ArmazenaDados.SaveList(pathListOperadoresFile, listOperadores);
                }
                else
                {
                    logger.Trace($"Lista de operadores carregada: {listOperadores.Count} operadores");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao carregar arquivos: " + ex);
            }

        }

        private static string ExibeMenu1()
        {
            try
            {
                bool isValidOption = false;
                string opcaoUsuario = "";

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
                    Console.WriteLine("E- Listar operadores");
                    Console.WriteLine("F- Inserir Operador");
                    Console.WriteLine("G- Remover Operador");
                    Console.WriteLine("H- Alterar senha do Operador");
                    Console.WriteLine();
                    Console.WriteLine("L- Limpar Tela");
                    Console.WriteLine("S- Sair/Fechar");
                    Console.WriteLine("Z- Fazer Logoff");
                    //todo implementar menu com setas

                    Console.WriteLine();
                    opcaoUsuario = EeS.ReadConsoleLine().ToUpper();

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

                        case "E":
                            isValidOption = true;
                            ListarOperadores();
                            break;
                        case "F":
                            isValidOption = true;
                            InserirOperador();
                            break;
                        case "G":
                            isValidOption = true;
                            ExcluirOperador();
                            break;
                        case "H":
                            isValidOption = true;
                            AlterarSenhaDoOperador();
                            break;


                        case "L":
                            isValidOption = true;
                            Console.Clear();
                            break;
                        case "S":
                            isValidOption = true;
                            Environment.Exit(0);
                            break;
                        case "Z":
                            //logoff
                            isValidOption = true;
                            logger.Trace($"Operador [{operadorLogado.Nome}] fez logoff.");
                            operadorLogado = null;
                            break;
                        default:
                            isValidOption = false;
                            Console.WriteLine("Opção inválida!");
                            break;
                    }

                } while (isValidOption == false);

                return opcaoUsuario;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return "";
            }
        }

        private static void ListarOperadores()
        {
            try
            {
                Console.WriteLine("Listar operadores:");

                if (listOperadores.Count == 0)
                {
                    Console.WriteLine("Nenhum operador cadastrado.");
                    return;
                }

                for (int i = 0; i < listOperadores.Count; i++)
                {
                    Operador operador = listOperadores[i];
                    Console.WriteLine(operador);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void AlterarSenhaDoOperador()
        {
            try
            {
                Console.WriteLine("Alterar a Senha Do Operador");

                string senhaAntiga;
                if (!operadorLogado.SolicitarSenha(out senhaAntiga))
                {
                    Console.WriteLine("Senha incorreta!");
                    return;
                }

                Console.Write("Insira a nova senha: ");
                String senhaNova = EeS.ReadConsoleLine();

                if (operadorLogado.AlterarSenha(senhaAntiga, senhaNova))
                {
                    //salva o arquivo incluindo a nova conta
                    ArmazenaDados.SaveList(pathListOperadoresFile, listOperadores);
                    logger.Trace($"Senha do operador [{operadorLogado.Usuario} - {operadorLogado.Nome} ] alterada com sucesso!");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// solicita dados e exclui operador
        /// </summary>
        private static void ExcluirOperador()
        {
            try
            {
                Console.WriteLine("Excluir Operador");
                Operador operador = null;

                Console.WriteLine("Digite o usuário a ser excluído: ");
                string entradaUsuario = EeS.ReadConsoleLine();

                if (operadorLogado.Usuario.Equals(entradaUsuario))
                {
                    Console.WriteLine("Você não pode excluir a si mesmo.");
                    //todo incluir logoff automático em caso se autoexclusao
                    return;
                }

                if ((operador = Operador.BuscaOperador(pListOperadores: listOperadores,
                                                            pUsuario: entradaUsuario,
                                                            pVerboseForAvailability: false))
                                                            == null)
                {
                    return;
                }

                if (!operadorLogado.SolicitarSenha())
                {
                    return;
                }

                if (listOperadores.Remove(operador))
                {
                    //salva o arquivo incluindo o novo operador
                    ArmazenaDados.SaveList(pathListOperadoresFile, listOperadores);
                    logger.Trace($"Operador [{operador.Usuario}] removido com sucesso!");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Solicita dados e insere novo operador
        /// </summary>
        private static void InserirOperador()
        {
            try
            {
                Console.WriteLine("Inserir novo operador");

                Console.WriteLine("Digite o usuário: ");
                string entradaUsuario = EeS.ReadConsoleLine();

                bool operadorDisponivel = false;
                Operador operador = null;
                do
                {
                    operadorDisponivel = true;
                    if ((operador = Operador.BuscaOperador(pListOperadores: listOperadores,
                                                            entradaUsuario,
                                                            pVerboseForAvailability: true))
                                                            != null)
                    {
                        operadorDisponivel = false;
                        Console.WriteLine("Tente outro usuário: ");
                        entradaUsuario = EeS.ReadConsoleLine();
                    }
                } while (operadorDisponivel == false);

                Console.Write("Digite o Nome do Operador: ");
                string entradaNome = EeS.ReadConsoleLine();

                Console.Write("Crie a senha do operador: ");
                String entradaSenha = EeS.ReadConsoleLine();

                Operador novoOperador = new Operador(pUsuario: entradaUsuario,
                                            pSenha: entradaSenha,
                                            pNome: entradaNome);

                listOperadores.Add(novoOperador);
                //salva o arquivo incluindo o novo operador
                ArmazenaDados.SaveList(pathListOperadoresFile, listOperadores);
                logger.Trace("Operador Criado: " + novoOperador.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Solicita dados e altera a senha da conta de cliente
        /// </summary>
        private static void AlterarSenhaDeConta()
        {
            try
            {
                Console.WriteLine("Alterar Senha De Conta");

                Cliente objCliente = Cliente.PedeContaEBuscaCliente(listClientes, pVerboseForAvailability: false);
                if (objCliente == null)
                {
                    return;
                }

                Console.Write("Digite a senha atual: ");
                string senhaAntiga = EeS.ReadConsoleLine();
                //Validação de senha
                if (!Password.CompararSenhas(senhaAntiga, objCliente.Salt, objCliente.Senha))
                {
                    Console.WriteLine("Senha incorreta!");
                    return;
                }

                Console.Write("Crie a nova senha com 6 dígitos numéricos: ");
                String senhaNova = EeS.ReadConsoleLine();

                while (!Password.ValidaRegraSenha(senhaNova))
                {
                    Console.WriteLine("Por favor, insira uma senha numérica de 6 dígitos.");
                    senhaNova = EeS.ReadConsoleLine();
                }

                if (objCliente.AlterarSenha(senhaAntiga, senhaNova))
                {
                    //salva o arquivo incluindo a nova conta
                    ArmazenaDados.SaveList(pathListClientesFile, listClientes);
                    logger.Trace($"Senha da conta [{objCliente.NumConta} - {objCliente.Nome} ] alterada com sucesso!");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }//fim AlterarSenhaDeConta()

        /// <summary>
        /// Solicita dados e exclui conta da ListClientes
        /// </summary>
        private static void ExcluirConta()
        {
            try
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

                if (!operadorLogado.SolicitarSenha())
                {
                    return;
                }

                if (listClientes.Remove(cliente))
                {
                    //salva o arquivo incluindo a nova conta
                    ArmazenaDados.SaveList(pathListClientesFile, listClientes);
                    logger.Trace($"Conta [{cliente.NumConta}] removida com sucesso!");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Solicita dados e deposita valor
        /// </summary>
        private static void Depositar()
        {
            try
            {
                Console.WriteLine("Depositar");

                Cliente objCliente = Cliente.PedeContaEBuscaCliente(pListClientes: listClientes, pVerboseForAvailability: false);
                if (objCliente == null)
                {
                    return;
                }

                double valorDeposito = EeS.PedeEvalidaDouble("Digite o valor a ser depositado: ");

                if (!operadorLogado.SolicitarSenha())
                {
                    return;
                }

                objCliente.Depositar(valorDeposito);
                //salva o arquivo incluindo a nova conta
                ArmazenaDados.SaveList(pathListClientesFile, listClientes);
                logger.Trace($"Depósito de {valorDeposito} na conta [{objCliente.NumConta} - {objCliente.Nome}] realizado com sucesso!");

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Solicita dados e realiza saque
        /// </summary>
        private static void Sacar()
        {
            try
            {
                Console.WriteLine("Sacar");
                Cliente objConta = Cliente.PedeContaEBuscaCliente(listClientes, pVerboseForAvailability: false);
                if (objConta == null)
                {
                    return;
                }

                Console.Write("Digite o valor a ser sacado: ");
                double valorSaque = double.Parse(EeS.ReadConsoleLine());

                Console.Write("Digite a senha: ");
                string senha = EeS.ReadConsoleLine();

                if (objConta.Sacar(valorSaque, senha))
                {
                    //salva o arquivo incluindo a nova conta
                    ArmazenaDados.SaveList(pathListClientesFile, listClientes);
                    logger.Trace($"Saque de {valorSaque} realizado na conta [{objConta.NumConta} - {objConta.Nome}]");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Solicita dados e executa transferência entre contas
        /// </summary>
        private static void Transferir()
        {
            try
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
                string senha = EeS.ReadConsoleLine();

                if (clienteOrigem.Transferir(senha, valorTransferencia, clienteDestino))
                {
                    //salva o arquivo incluindo a nova conta
                    ArmazenaDados.SaveList(pathListClientesFile, listClientes);
                    logger.Trace($"Transferência de {valorTransferencia} realizada de conta [{clienteOrigem.NumConta} - {clienteOrigem.Nome}] para a conta [{clienteDestino.NumConta} - {clienteDestino.Nome}]");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Solicita dados e insere nova conta de cliente.
        /// </summary>
        private static void InserirConta()
        {
            try
            {
                Console.WriteLine("Inserir nova conta");

                int entradaTipoConta = EeS.PedeEvalidaInteger("Digite 1 para Conta Física ou 2 para Jurídica: ");

                int entradaNumeroConta = EeS.PedeEvalidaInteger("Insira o número desejado para a conta: ");

                bool contaDisponivel = false;
                Cliente cliente = null;
                do
                {
                    contaDisponivel = true;
                    if ((cliente = Cliente.BuscaCliente(pListContas: listClientes,
                                                                entradaNumeroConta,
                                                                pVerboseForAvailability: true))
                                                                != null)
                    {
                        contaDisponivel = false;
                        entradaNumeroConta = EeS.PedeEvalidaInteger("Insira outro número para a conta: ");
                    }
                } while (contaDisponivel == false);

                Console.Write("Digite o Nome do Cliente: ");
                string entradaNome = EeS.ReadConsoleLine();

                double entradaSaldo = EeS.PedeEvalidaDouble("Digite o saldo inicial: ");

                double entradaCredito = EeS.PedeEvalidaDouble("Digite o crédito concedido: ");

                Console.Write("Crie a senha com 6 dígitos numéricos: ");
                String entradaSenha = EeS.ReadConsoleLine();

                while (!Password.ValidaRegraSenha(entradaSenha))
                {
                    Console.WriteLine("Por favor, insira uma senha numérica de 6 dígitos.");
                    entradaSenha = EeS.ReadConsoleLine();
                }

                Cliente novaConta = new Cliente(pTipoConta: (TipoConta)entradaTipoConta,
                                            pSaldo: entradaSaldo,
                                            pCredito: entradaCredito,
                                            pNome: entradaNome,
                                            pNumConta: entradaNumeroConta,
                                            pSenha: entradaSenha);

                listClientes.Add(novaConta);
                //salva o arquivo incluindo a nova conta
                ArmazenaDados.SaveList(pathListClientesFile, listClientes);
                logger.Trace("Conta Criada: " + novaConta.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Lista contas em ListClientes
        /// </summary>
        private static void ListarContas()
        {
            try
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
            catch (Exception ex)
            {
                logger.Error(ex);
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
            try
            {
                string[] fileSplit = filePath.Split("\\");
                string fileName = fileSplit[fileSplit.Length - 1];
                return lineNumber + "|" + caller + "|" + fileName + "|";
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return "";
            }
        }

    }//fim do programa
}//fim do namespace
