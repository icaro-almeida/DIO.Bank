using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIO.Bank
{
    [Serializable]
    class Operador : Usuario
    {

        public string Usuario { get; private set; }

        /// <summary>
        /// Construtor da classe Operador
        /// </summary>
        /// <param name="pUsuario"></param>
        /// <param name="pSenha"></param>
        /// <param name="pNome"></param>
        public Operador(string pUsuario, string pSenha, string pNome) : base(pNome, pSenha)
        {
            this.Usuario = pUsuario;
        }

        /// <summary>
        /// Busca operador na lista por usuário
        /// </summary>
        /// <param name="pListOperadores"></param>
        /// <param name="pUsuario"></param>
        /// <param name="pVerboseForAvailability"></param>
        /// <returns>True se o operador foi encontrado</returns>
        internal static Operador BuscaOperador(List<Operador> pListOperadores, string pUsuario, bool pVerboseForAvailability = false)
        {
            List<Operador> resultsList = pListOperadores.FindAll(x => (x.Usuario == pUsuario));

            if (resultsList.Count == 1)
            {
                if (pVerboseForAvailability)
                    Console.WriteLine($"Operador [{pUsuario}] já cadastrado!");
                else
                    Console.WriteLine($"Operador [{pUsuario}] encontrado: {resultsList[0].Nome}");
                return resultsList[0];
            }
            else
            {
                if (pVerboseForAvailability)
                    Console.WriteLine($"Operador [{pUsuario}] disponível!");
                else
                    Console.WriteLine($"Operador [{pUsuario}] não encontrado!");
                return null;
            }

        }

        /// <summary>
        /// Exibe login do operador e pede usuário e senha
        /// </summary>
        /// <param name="pListOperadores">Lista de operadores</param>
        /// <returns></returns>
        public static Operador ExecutaLoginOperador(List<Operador> pListOperadores)
        {
            Operador operador = null;
            string user = null;
            do
            {
                Console.WriteLine();
                Console.WriteLine("---- LOGIN DO OPERADOR ----");
                Console.Write("Usuário: ");
                user = EeS.ReadConsoleLine();
                operador = Operador.BuscaOperador(pListOperadores, user);
            } while (operador == null);

            string pass = null;
            bool senhaCorreta = true;
            do
            {
                if (!senhaCorreta)
                {
                    Console.WriteLine("Senha incorreta!");
                }
                Console.Write("Senha: ");
                pass = EeS.ReadConsoleLine();
            } while (!(senhaCorreta = Password.CompararSenhas(pass, operador.Salt, operador.Senha)));

            return operador;
        }

        /// <summary>
        /// Solicita senha ao operador e verifica
        /// </summary>
        /// <returns>True se a senha digitada confere</returns>
        internal bool SolicitarSenha()
        {
            Console.WriteLine($"Operador [{this.Nome}], insira a senha: ");
            string senha = EeS.ReadConsoleLine();

            bool senhasConferem = false;
            if (!(senhasConferem = Password.CompararSenhas(senha, this.Salt, this.Senha)))
            {
                Console.WriteLine("Senha incorreta!");
            }

            return senhasConferem;
        }

        /// <summary>
        /// Solicita senha ao operador e verifica. Retorna via  out a senha digitada.
        /// </summary>
        /// <param name="senha"></param>
        /// <returns>True se as senhas conferem</returns>
        internal bool SolicitarSenha(out string senha)
        {
            Console.WriteLine($"Operador [{this.Nome}], insira a senha: ");
            senha = EeS.ReadConsoleLine();

            bool senhasConferem = false;
            if (!(senhasConferem = Password.CompararSenhas(senha, this.Salt, this.Senha)))
            {
                Console.WriteLine("Senha incorreta!");
            }

            return senhasConferem;
        }

        /// <summary>
        /// Gera string com os dados do Cliente
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retorno = "";
            retorno += "Usuário [" + this.Usuario + "] | ";            
            retorno += "Nome " + this.Nome + " | ";         

            return retorno;
        }

    }//fim da classe
}
