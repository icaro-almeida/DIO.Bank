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
				Console.WriteLine("Usuário: ");
				user = Console.ReadLine();				
				operador = Operador.BuscaOperador(pListOperadores, user);
			} while (operador == null);

			string pass = null;
			do
			{
				Console.WriteLine("Senha:");
				pass = Console.ReadLine();				
			} while (!Password.CompararSenhas(pass, operador.Salt, operador.Senha));

			return operador;
		}

	}//fim da classe
}
