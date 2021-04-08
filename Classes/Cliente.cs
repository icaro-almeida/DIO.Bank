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

        /// <summary>
        /// Valida e executa saque
        /// </summary>
        /// <param name="pValorSaque">Valor a ser sacado</param>
        /// <param name="pSenha">Senha digitada pelo cliente para autorização da operação</param>
        /// <returns></returns>
        public bool Sacar(double pValorSaque, string pSenha)
        {
            // Validação de saldo suficiente
            if (this.Saldo - pValorSaque < (this.Credito * -1))
            {
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

            //Exibe saldo atual
            Console.WriteLine($"Conta [{this.NumConta}] - Saldo atual: {this.Saldo}");
            // https://docs.microsoft.com/pt-br/dotnet/standard/base-types/composite-formatting

            return true;
        }

        /// <summary>
        /// Insere dinheiro na conta do cliente
        /// </summary>
        /// <param name="valorDeposito"></param>
        public void Depositar(double valorDeposito)
        {
            this.Saldo += valorDeposito;
        }

        public bool Transferir(string pSenha, double valorTransferencia, Cliente contaDestino)
        {
            if (this.Sacar(valorTransferencia, pSenha))
            {
                contaDestino.Depositar(valorTransferencia);
                return true;
            }
            return false;
        }

        ///<summary>Busca objeto na lista Conta por agência e conta</summary>
        internal static Cliente BuscaConta(List<Cliente> pListContas, int pConta, bool pVerboseForAvailability = true)
        {
            
        }

        internal static Cliente PedeContaEBuscaCliente(List<Cliente> pListClientes, string pMsg = "Digite o número da conta: ", bool pVerboseForAvailability = true)
        {
            Console.Write(pMsg);
            int inputConta = int.Parse(Console.ReadLine());

            //List<Conta> resultsList = pListContas.FindAll(x => (x.NumConta == pConta) && (x.NumAgencia == pAgencia));		
            List<Cliente> resultsList = pListClientes.FindAll(x => (x.NumConta == inputConta));

            if (resultsList.Count == 1)
            {
                if (pVerboseForAvailability)
                    Console.WriteLine($"Conta [{inputConta}] já cadastrada!");
                else
                    Console.WriteLine($"Conta [{inputConta}] encontrada!");

                return resultsList[0];
            }
            else
            {
                if (pVerboseForAvailability)
                    Console.WriteLine($"Conta [{inputConta}] disponível!");
                else
                    Console.WriteLine($"Conta [{inputConta}] não encontrada!");

                return null;
            }
        }

        public override string ToString()
        {
            string retorno = "";
            retorno += "Conta [" + this.NumConta + "] | ";
            retorno += "TipoConta " + this.TipoConta + " | ";
            retorno += "Nome " + this.Nome + " | ";
            retorno += "Saldo " + this.Saldo + " | ";
            retorno += "Crédito " + this.Credito + " | ";

            return retorno;
        }
    }
}