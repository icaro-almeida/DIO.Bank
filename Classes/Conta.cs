using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DIO.Bank
{
	[Serializable]
	public class Conta
	{
		// Atributos
		private TipoConta TipoConta { get; set; }
		private double Saldo { get; set; }
		private double Credito { get; set; }
		private string Nome { get; set; }
		public int NumAgencia { get; set; }
		public int NumConta { get; set; }

		private byte[] _senha;
		/// <summary>The Senha property represents the user's password.</summary>
		/// <value>The Senha property gets/sets the value of the byte[] field, _senha.</value>
		public byte[] Senha
		{
			get
			{
				return _senha;
			}
			set
			{
				_senha = value;
			}
		}

		private string Salt { get; set; }

		// Métodos
		public Conta(TipoConta pTipoConta, double pSaldo, double pCredito, string pNome,
		 int pNumAgencia, int pNumConta, string pSenha)
		{
			this.TipoConta = pTipoConta;
			this.Saldo = pSaldo;
			this.Credito = pCredito;
			this.Nome = pNome;
			this.NumAgencia = pNumAgencia;
			this.NumConta = pNumConta;

			this.Salt = Password.CreateSalt(8);
			this.Senha = Password.GenerateSaltedHash(pSenha,this.Salt);
		}

		public bool Sacar(double pValorSaque, string pSenha)
		{
            // Validação de saldo suficiente
            if (this.Saldo - pValorSaque < (this.Credito *-1)){
                Console.WriteLine("Saldo insuficiente!");
                return false;
            }

			;//Validação de senha
            if (!Password.CompararSenhas(pSenha, this.Salt, this.Senha))
            {
				Console.WriteLine("Senha incorreta!");
				return false;
            }

			//executa retirada
            this.Saldo -= pValorSaque;

			//Exibe saldo atual
            Console.WriteLine("Saldo atual da conta de {0} é {1}", this.Nome, this.Saldo);
            // https://docs.microsoft.com/pt-br/dotnet/standard/base-types/composite-formatting

            return true;
		}

		public void Depositar(double valorDeposito)
		{
			this.Saldo += valorDeposito;

            Console.WriteLine("Saldo atual da conta de {0} é {1}", this.Nome, this.Saldo);
		}        

        public void Transferir(string pSenha, double valorTransferencia, Conta contaDestino)
		{
			if (this.Sacar(valorTransferencia, pSenha)){
                contaDestino.Depositar(valorTransferencia);
            }
		}

		///<summary>Busca objeto na lista Conta por agência e conta</summary>
		internal static Conta BuscaConta(List<Conta> pListContas, int pAgencia, int pConta)
        {
			
			List<Conta> resultsList = pListContas.FindAll(x => (x.NumConta == pAgencia) && (x.NumAgencia == pConta));
			
			if (resultsList.Count == 1)
			{
				return resultsList[0];
			}
			else
			{
				Console.WriteLine("Contas duplicadas encontradas:");
				foreach (var item in resultsList)
				{
					Console.WriteLine(item);
				}
				Console.WriteLine("Procure sua agência!");
				return null;
			}			
        }

		internal static Conta PedeAgenciaConta(List<Conta> listContas)
		{
			Console.Write("Digite o número da agência: ");
			int agencia = int.Parse(Console.ReadLine());

			Console.Write("Digite o número da conta: ");
			int conta = int.Parse(Console.ReadLine());

			return Conta.BuscaConta(listContas, agencia, conta);
		}

		public override string ToString()
		{
            string retorno = "";
            retorno += "TipoConta " + this.TipoConta + " | ";
            retorno += "Nome " + this.Nome + " | ";
            retorno += "Saldo " + this.Saldo + " | ";
            retorno += "Crédito " + this.Credito + " | ";
			retorno += "Agência " + this.NumAgencia + " | ";
			retorno += "Conta " + this.NumConta + " | ";
			return retorno;
		}
	}
}