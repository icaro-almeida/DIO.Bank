using System;
using System.Collections.Generic;

namespace DIO.Bank
{
	class Program
	{
		static List<Conta> listContas = new List<Conta>();
		static void Main(string[] args)
		{						
			//Soluciona problema de diretório variável entre "dotnet run", vscode e visual studio
			String diretorioBase = AppDomain.CurrentDomain.BaseDirectory;
			//https://jeremybytes.blogspot.com/2020/02/set-working-directory-in-visual-studio.html

			listContas = ArmazenaDados.LoadList<Conta>(diretorioBase + "first.dat");
			if(listContas.Count == 0)
				Console.WriteLine("Não foi possível carregar a lista de contas!");
			else
				Console.WriteLine("Lista de contas carregada com sucesso!");			

			string opcaoUsuario = ObterOpcaoUsuario();

			while (opcaoUsuario.ToUpper() != "X")
			{
				switch (opcaoUsuario)
				{
					case "1":
						ListarContas();
						break;
					case "2":
						InserirConta();
						break;
					case "3":
						Transferir();
						break;
					case "4":
						Sacar();
						break;
					case "5":
						Depositar();
						break;
                    case "C":
						Console.Clear();
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}

				opcaoUsuario = ObterOpcaoUsuario();
			}
			
			Console.WriteLine("Obrigado por utilizar nossos serviços.");

			//Salva dados de contas antes de encerrar:
			ArmazenaDados.SaveList(diretorioBase + "first.dat", listContas);
			//mantém console aberto até que pressionem uma tecla:
			Console.ReadLine();				
		}

		private static void Depositar()
		{
			Console.Write("Digite o número da conta: ");
			int indiceConta = int.Parse(Console.ReadLine());

			Console.Write("Digite o valor a ser depositado: ");
			double valorDeposito = double.Parse(Console.ReadLine());

            listContas[indiceConta].Depositar(valorDeposito);
		}

		private static void Sacar()
		{
			Conta objConta = Conta.PedeAgenciaConta(listContas);
			if (objConta == null)
            {
				Console.WriteLine("Conta inválida!");
				return;
			}				

			Console.Write("Digite o valor a ser sacado: ");
			double valorSaque = double.Parse(Console.ReadLine());

			Console.Write("Digite a senha: ");
			string senha = Console.ReadLine();			

			objConta.Sacar(valorSaque,senha);
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

			listContas[indiceContaOrigem].Transferir(senha, valorTransferencia, listContas[indiceContaDestino]);
		}

		private static void InserirConta()
		{
			Console.WriteLine("Inserir nova conta");

			Console.Write("Digite 1 para Conta Física ou 2 para Jurídica: ");
			int entradaTipoConta = int.Parse(Console.ReadLine());

			Console.Write("Digite o Nome do Cliente: ");
			string entradaNome = Console.ReadLine();

			Console.Write("Digite o número da agência: ");
			int entradaAgencia = int.Parse(Console.ReadLine());

			Console.Write("Digite o número da conta: ");
			int entradaConta = int.Parse(Console.ReadLine());									

			Console.Write("Digite o saldo inicial: ");
			double entradaSaldo = double.Parse(Console.ReadLine());

			Console.Write("Digite o crédito: ");
			double entradaCredito = double.Parse(Console.ReadLine());

			Console.Write("Digite a senha de 6 dígitos numéricos: ");
			String entradaSenha = Console.ReadLine();

			while (!Password.ValidaSenha(entradaSenha))
			{
				Console.WriteLine("Por favor, insira uma senha numérica de 6 dígitos.");
				entradaSenha = Console.ReadLine();
			} 

			Conta novaConta = new Conta(pTipoConta: (TipoConta)entradaTipoConta,
										pSaldo: entradaSaldo,
										pCredito: entradaCredito,
										pNome: entradaNome,
										pNumAgencia: entradaAgencia,
										pNumConta: entradaConta,
										pSenha: entradaSenha);

			listContas.Add(novaConta);
		}

		private static void ListarContas()
		{
			Console.WriteLine("Listar contas");

			if (listContas.Count == 0)
			{
				Console.WriteLine("Nenhuma conta cadastrada.");
				return;
			}

			for (int i = 0; i < listContas.Count; i++)
			{
				Conta conta = listContas[i];
				Console.Write("#{0} - ", i);
				Console.WriteLine(conta);
			}
		}

		private static string ObterOpcaoUsuario()
		{
			Console.WriteLine();
			Console.WriteLine("DIO Bank a seu dispor!!!");
			Console.WriteLine("Informe a opção desejada:");

			Console.WriteLine("1- Listar contas");
			Console.WriteLine("2- Inserir nova conta");
			Console.WriteLine("3- Transferir");
			Console.WriteLine("4- Sacar");
			Console.WriteLine("5- Depositar");
            Console.WriteLine("C- Limpar Tela");
			Console.WriteLine("X- Sair");
			Console.WriteLine();

			string opcaoUsuario = Console.ReadLine().ToUpper();
			Console.WriteLine();
			return opcaoUsuario;
		}

	}
}
