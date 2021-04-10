using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIO.Bank
{
	[Serializable]
	public class Usuario
    {
        public string Nome { get; private set; }

		private byte[] _senha;
		/// <summary>The Senha property represents the user's password.</summary>
		/// <value>The Senha property gets/sets the value of the byte[] field, _senha.</value>
		public byte[] Senha
		{
			get
			{
				return _senha;
			}
			private set
			{
				_senha = value;
			}
		}

		public string Salt { get; private set; }

		public Usuario(string pNome, string pSenha)
        {
			this.Nome = pNome;
			this.Salt = Password.CreateSalt(8);
			this.Senha = Password.GenerateSaltedHash(pSenha, this.Salt);
		}

		/// <summary>
		/// Verifica senha antiga inserida e troca pela senha nova
		/// </summary>
		/// <param name="pSenhaAntiga"></param>
		/// <param name="pSenhaNova"></param>
		/// <returns></returns>
		public bool AlterarSenha(string pSenhaAntiga, string pSenhaNova)
        {
			if (!Password.CompararSenhas(pSenhaAntiga, this.Salt, this.Senha))
			{
				Console.WriteLine("Senha incorreta!");
				return false;
			}

			this.Senha = Password.GenerateSaltedHash(pSenhaNova, this.Salt);
			return true;
		}



	}//fim da classe
}// fim do namespace
