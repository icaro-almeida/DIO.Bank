using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DIO.Bank
{
	[Serializable]
	public class Cliente : Usuario
	{
		// Atributos
		private TipoConta TipoConta { get; set; }
		private double Saldo { get; set; }
		private double Credito { get; set; }
		public int NumConta { get; set; }			

		// Métodos
		public Cliente(TipoConta pTipoConta, double pSaldo, double pCredito, string pNome,
		 int pNumConta, string pSenha) : base(pNome, pSenha)
		{			
			this.TipoConta = pTipoConta;
			this.Saldo = pSaldo;
			this.Credito = pCredito;					
			this.NumConta = pNumConta;			
		}

		public bool Sacar(double pValorSaque, string pSenha)
		{
            // Validação de saldo suficiente
            if (this.Saldo - pValorSaque < (this.Credito *-1)){
                Console.WriteLine("Saldo insuficiente!");
                return false;
            }

			//Validação de senha
            if (!Password.CompararSenhas(pSenha, this.Salt, this.Senha))
            {
				Console.WriteLine("Senha incorreta!");
				return false;
            }

			//executa retirada
            this.Saldo -= pValorSaque;

			//todo alterar mensagem abaixo
			//Exibe saldo atual
			Console.WriteLine($"Conta número {this.NumConta} de {this.Nome}");
			Console.WriteLine($"Saldo atual: {this.Saldo}");
            // https://docs.microsoft.com/pt-br/dotnet/standard/base-types/composite-formatting

            return true;
		}

		public void Depositar(double valorDeposito)
		{
			this.Saldo += valorDeposito;

			//todo remover esta mensagem e adicionar fora daqui. deixar aqui msg de depósito realizado.
            Console.WriteLine("Saldo atual da conta de {0} é {1}", this.Nome, this.Saldo);
		}

        public void Transferir(string pSenha, double valorTransferencia, Cliente contaDestino)
		{
			if (this.Sacar(valorTransferencia, pSenha)){
                contaDestino.Depositar(valorTransferencia);
            }
		}

		///<summary>Busca objeto na lista Conta por agência e conta</summary>
		internal static Cliente BuscaConta(List<Cliente> pListContas, int pConta)
        {
			//List<Conta> resultsList = pListContas.FindAll(x => (x.NumConta == pConta) && (x.NumAgencia == pAgencia));		
			List<Cliente> resultsList = pListContas.FindAll(x => (x.NumConta == pConta));

			if (resultsList.Count == 1)
			{
				return resultsList[0];
			}
            else if(resultsList.Count == 0)
			{
				Console.WriteLine($"Conta [{pConta}] disponível/não cadastrada!");
				return null;
			}
			else
			{
				Console.WriteLine("Contas duplicadas encontradas:");
				foreach (var item in resultsList)
				{
					Console.WriteLine(item);
				}
				Console.WriteLine("Informe ao suporte técnico!");
				return null;
			}
        }

		internal static Cliente PedeContaEBuscaCliente(List<Cliente> listClientes, string msg = "Digite o número da conta: ")
		{		
			Console.Write(msg);
			int conta = int.Parse(Console.ReadLine());

			return Cliente.BuscaConta(listClientes, conta);
		}

		public override string ToString()
		{
            string retorno = "";
            retorno += "TipoConta " + this.TipoConta + " | ";
            retorno += "Nome " + this.Nome + " | ";
            retorno += "Saldo " + this.Saldo + " | ";
            retorno += "Crédito " + this.Credito + " | ";
			retorno += "Conta " + this.NumConta + " | ";
			return retorno;
		}
	}
}