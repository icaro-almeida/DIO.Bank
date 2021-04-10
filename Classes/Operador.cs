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

        private string Usuario { get; set; }

        public Operador(string pUsuario, string pSenha, string pNome) : base(pNome, pSenha)
        {
            this.Usuario = pUsuario;
        }

        ///<summary>Busca objeto na lista Conta por agência e conta</summary>
        internal static Operador BuscaOperador(List<Operador> pListOperadores, string pUsuario)
        {
            List<Operador> resultsList = pListOperadores.FindAll(x => (x.Usuario == pUsuario));

            if (resultsList.Count == 1)
            {
                return resultsList[0];
            }
            else if (resultsList.Count == 0)
            {
                Console.WriteLine($"Operador [{pUsuario}] disponível/não cadastrado!");
                return null;
            }
            else
            {
                Console.WriteLine("Operadores duplicados encontrados:");
                foreach (var item in resultsList)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("Informe ao suporte técnico!");
                return null;
            }
        }

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

    }//fim da classe
}
