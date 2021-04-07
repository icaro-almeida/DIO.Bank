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

		static string pathListClientes = diretorioBase + "listClientes.dat";
		static string pathListOperadores = diretorioBase + "listOperadores.dat";

		static Operador operadorLogado = null;

		static void Main(string[] args)
		{			
			CarregaDados();

			//Cria usuário admin caso não encontre arquivo de operadores
			if (listOperadores.Count == 0)
            {
				listOperadores.Add(new Operador("admin", "admin", "admin"));
				//Cria o arquivo que salva os usuários operadores com novo operador Admin
				ArmazenaDados.SaveList(pathListOperadores, listOperadores);
			}		

			//Exibe menus
			do
            {
				operadorLogado = Operador.ExecutaLoginOperador(listOperadores);
			} while (ExibeMenu1() != "S");	

			//Salva dados de contas e operadores antes de encerrar:
			ArmazenaDados.SaveList(pathListClientes, listClientes);
			ArmazenaDados.SaveList(pathListOperadores, listOperadores);

			//mantém console aberto até que pressionem uma tecla:
			Console.ReadLine();
		}      

        /// <summary>
        /// Carrega dados de clientes e operadores
        /// </summary>
        public static void CarregaDados()
		{		
			//Carrega lista de clientes/contas
			listClientes = ArmazenaDados.LoadList<Cliente>(pathListClientes);
			if (listClientes.Count == 0)
			{
				string msg = "Não foi possível carregar a lista de contas!";
				Console.WriteLine(msg);
				logger.Error(msg);
			}
			else
			{
				string msg = "Lista de contas carregada com sucesso!";
				Console.WriteLine(msg);
				logger.Info(msg);
			}

			//carrega lista de operadores
			listOperadores = ArmazenaDados.LoadList<Operador>(pathListOperadores);
			if (listOperadores.Count == 0)
			{
				string msg = "Não foi possível carregar a lista de operadores!";
				Console.WriteLine(msg);
				logger.Error(msg);
			}
			else
			{
				string msg = "Lista de operadores carregada com sucesso!";
				Console.WriteLine(msg);
				logger.Info(msg);
			}

		}

		private static string ExibeMenu1()
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
					Console.WriteLine("D- Trocar senha de conta");
					Console.WriteLine();
					Console.WriteLine("E- Inserir Operador");
					Console.WriteLine("F- Remover Operador");
					Console.WriteLine("G- Trocar senha de Operador");
					Console.WriteLine();
					Console.WriteLine("L- Limpar Tela");
					Console.WriteLine("S- Sair");

					//EDIT SWITCH-CASE BELOW FOR EACH POSSIBLE OPTION					
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

        private static void AlterarSenhaDeConta()
        {
			Cliente objCliente = Cliente.PedeContaEBuscaCliente(listClientes);
			if (objCliente == null)
			{				
				return;
			}

			Console.Write("Digite a senha atual: ");
			string senha = Console.ReadLine();
			//Validação de senha
			if (!Password.CompararSenhas(senha, objCliente.Salt, objCliente.Senha))
			{
				Console.WriteLine("Senha incorreta!");
				return;
			}

			Console.Write("Crie a nova senha com 6 dígitos numéricos: ");
			String entradaSenha = Console.ReadLine();

			while (!Password.ValidaRegraSenha(entradaSenha))
			{
				Console.WriteLine("Por favor, insira uma senha numérica de 6 dígitos.");
				entradaSenha = Console.ReadLine();
			}

			objCliente.AlteraSenha(entradaSenha);

			string msg = $"Senha da conta {objCliente.NumConta} alterada com sucesso!";
			Console.WriteLine(msg);
			logger.Info(msg);
		}

		/// <summary>
		/// Solicita dados e exclui conta da ListClientes
		/// </summary>
        private static void ExcluirConta()
        {
			int numConta;
			Console.WriteLine("Digite o número da conta a ser excluída: ");
			numConta = int.Parse(Console.ReadLine());
			Cliente cliente;

            if ((cliente = Cliente.BuscaConta(listClientes, numConta)) == null)
            {
				Console.WriteLine($"Conta [{numConta}] inexistente!");				
				return;
			}

			listClientes.Remove(cliente);
			string msg = $"Conta {numConta} removida com sucesso!";
			Console.WriteLine(msg);
			logger.Info(msg);
		}

        private static void Depositar()
		{
			Console.Write("Digite o número da conta: ");
			int numConta = int.Parse(Console.ReadLine());

			if (Cliente.BuscaConta(listClientes, numConta) == null)
			{
				Console.WriteLine($"Conta [{numConta}] inexistente!");
				return;
			}

			Console.Write("Digite o valor a ser depositado: ");
			double valorDeposito = double.Parse(Console.ReadLine());

			listClientes[numConta].Depositar(valorDeposito);
			string msg = $"Depósito de {valorDeposito} na conta {numConta} realizado com sucesso!";
			Console.WriteLine(msg);
			logger.Info(msg);
		}

		/// <summary>
		/// Realiza saque
		/// </summary>
		private static void Sacar()
		{
			Cliente objConta = Cliente.PedeContaEBuscaCliente(listClientes);
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
				logger.Info($"Saque de {valorSaque} realizado na conta {objConta.NumConta}");
			}

		}

		private static void Transferir()
		{
			//TODO
			//Inserir pedido de agência e conta
			Console.Write("Digite o número da conta de origem: ");
			int indiceContaOrigem = int.Parse(Console.ReadLine());

			Console.Write("Digite o número da conta de destino: ");
			int indiceContaDestino = int.Parse(Console.ReadLine());

			Console.Write("Digite o valor a ser transferido: ");
			double valorTransferencia = double.Parse(Console.ReadLine());

			Console.Write("Digite a senha: ");
			string senha = Console.ReadLine();

			//todo corrigir a referencia do objeto conta abaixo que ainda está usnado índice diretamente

			listClientes[indiceContaOrigem].Transferir(senha, valorTransferencia, listClientes[indiceContaDestino]);
		}

		/// <summary>
		/// Solicita dados e insere nova conta de cliente.
		/// </summary>
		private static void InserirConta()
		{
			Console.WriteLine("Inserir nova conta");

			Console.Write("Digite 1 para Conta Física ou 2 para Jurídica: ");
			int entradaTipoConta = int.Parse(Console.ReadLine());

			int entradaNumeroConta = 0;
			bool contaDisponivel = false;
            do
            {
				//todo alterar mensagem de "conta nao encontrada ao verificar disponibilidade da conta"
				contaDisponivel = true;
				Console.Write("Insira o número desejado para a conta: ");
				entradaNumeroConta = int.Parse(Console.ReadLine());
				if(Cliente.BuscaConta(listClientes, entradaNumeroConta) != null)                
					contaDisponivel = false;
			} while (contaDisponivel == false);
			

			Console.Write("Digite o Nome do Cliente: ");
			string entradaNome = Console.ReadLine();			

			Console.Write("Digite o saldo inicial: ");
			double entradaSaldo = double.Parse(Console.ReadLine());

			Console.Write("Digite o crédito concedido: ");
			double entradaCredito = double.Parse(Console.ReadLine());

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
			//todo adicionar saída para o console
			logger.Info("Nova Conta Adicionada: " + novaConta.ToString());
		}

		private static void ListarContas()
		{
			Console.WriteLine("Listar contas");

			if (listClientes.Count == 0)
			{
				Console.WriteLine("Nenhuma conta cadastrada.");
				return;
			}

			for (int i = 0; i < listClientes.Count; i++)
			{
				Cliente conta = listClientes[i];
				Console.Write("#{0} - ", i);
				Console.WriteLine(conta);
			}
		}

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
