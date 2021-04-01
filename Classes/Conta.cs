using System;

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
		private int _NumAgencia { get; set; }			
		public int NumAgencia
		{
			get
			{
				return _NumAgencia;
			}
			set
			{
				_NumAgencia = value;
			}
		}
		private int _NumConta { get; set; }	
		public int NumConta
		{
			get
			{
				return _NumConta;
			}
			set
			{
				_NumConta = value;
			}
		}


		private int Senha { get; set; }

		// Métodos
		public Conta(TipoConta pTipoConta, double pSaldo, double pCredito, string pNome,
		 int pNumAgencia, int pNumConta, int pSenha)
		{
			this.TipoConta = pTipoConta;
			this.Saldo = pSaldo;
			this.Credito = pCredito;
			this.Nome = pNome;
			this.NumAgencia = pNumAgencia;
			this.NumConta = pNumConta;
			this.Senha = pSenha;
		}

		public bool Sacar(double valorSaque)
		{
            // Validação de saldo suficiente
            if (this.Saldo - valorSaque < (this.Credito *-1)){
                Console.WriteLine("Saldo insuficiente!");
                return false;
            }
            this.Saldo -= valorSaque;

            Console.WriteLine("Saldo atual da conta de {0} é {1}", this.Nome, this.Saldo);
            // https://docs.microsoft.com/pt-br/dotnet/standard/base-types/composite-formatting

            return true;
		}

		public void Depositar(double valorDeposito)
		{
			this.Saldo += valorDeposito;

            Console.WriteLine("Saldo atual da conta de {0} é {1}", this.Nome, this.Saldo);
		}

		public void Transferir(double valorTransferencia, Conta contaDestino)
		{
			if (this.Sacar(valorTransferencia)){
                contaDestino.Depositar(valorTransferencia);
            }
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