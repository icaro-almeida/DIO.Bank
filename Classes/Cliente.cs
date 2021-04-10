using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DIO.Bank
{
    [Serializable]
    public class Cliente : Usuario
    {
        // Attributes
        private TipoConta TipoConta { get; set; }
        private double Saldo { get; set; }
        private double Credito { get; set; }
        public int NumConta { get; set; }

        // Methods
        /// <summary>
        /// Construtor da classe Cliente
        /// </summary>
        /// <param name="pTipoConta"></param>
        /// <param name="pSaldo"></param>
        /// <param name="pCredito"></param>
        /// <param name="pNome"></param>
        /// <param name="pNumConta"></param>
        /// <param name="pSenha"></param>
        public Cliente(TipoConta pTipoConta, double pSaldo, double pCredito, string pNome,
         int pNumConta, string pSenha) : base(pNome, pSenha)
        {
            this.TipoConta = pTipoConta;
            this.Saldo = pSaldo;
            this.Credito = pCredito;
            this.NumConta = pNumConta;
        }

        /// <summary>
        /// Valida senha, saldo e executa saque
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

        /// <summary>
        /// Realiza transferência e confere senha
        /// </summary>
        /// <param name="pSenha">Senha do cliente para validação da operação</param>
        /// <param name="valorTransferencia">Valor da transferência</param>
        /// <param name="contaDestino">Objeto do Cliente de destino</param>
        /// <returns></returns>
        public bool Transferir(string pSenha, double valorTransferencia, Cliente contaDestino)
        {
            if (this.Sacar(valorTransferencia, pSenha))
            {
                contaDestino.Depositar(valorTransferencia);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Busca e retorna Objeto de Cliente na List a partir do número de conta
        /// </summary>
        /// <param name="pListContas"></param>
        /// <param name="pConta"></param>
        /// <param name="pVerboseForAvailability"></param>
        /// <returns></returns>
        internal static Cliente BuscaCliente(List<Cliente> pListContas, int pConta, bool pVerboseForAvailability = false)
        {
            //List<Conta> resultsList = pListContas.FindAll(x => (x.NumConta == pConta) && (x.NumAgencia == pAgencia));		
            List<Cliente> resultsList = pListContas.FindAll(x => (x.NumConta == pConta));

            if (resultsList.Count == 1)
            {
                if (pVerboseForAvailability)
                    Console.WriteLine($"Conta [{pConta}] já cadastrada!");
                else
                    Console.WriteLine($"Conta [{pConta}] encontrada: {resultsList[0].Nome}");
                return resultsList[0];
            }
            else
            {
                if (pVerboseForAvailability)
                    Console.WriteLine($"Conta [{pConta}] disponível!");
                else
                    Console.WriteLine($"Conta [{pConta}] não encontrada!");

                return null;
            }

        }

        /// <summary>
        /// Solicita o número da conta e retorna Objeto de Cliente na List
        /// </summary>
        /// <param name="pListClientes">List de clientes</param>
        /// <param name="pMsg">Mensagem a ser exibida solicitando conta</param>
        /// <param name="pVerboseForAvailability">Exibir mensagens de resultado informando de disponível/indisponível ou encontrada/não encontrada</param>
        /// <returns></returns>
        internal static Cliente PedeContaEBuscaCliente(List<Cliente> pListClientes, string pMsg = "Digite o número da conta: ", bool pVerboseForAvailability = true)
        {
            int conta = EeS.PedeEvalidaInteger(pMsg);

            return Cliente.BuscaCliente(pListClientes, conta, pVerboseForAvailability);
        }

        /// <summary>
        /// Gera string com os dados do Cliente
        /// </summary>
        /// <returns></returns>
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