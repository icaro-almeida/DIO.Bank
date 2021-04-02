using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DIO.Bank
{
    class Password
    {

		/// <summary>Validates password rules:
		/// <list type="bullet">
		/// <item>
		/// <description>Password must contain 6 digits.</description>
		/// </item>
		/// <item>
		/// <description>Password must contain only numbers (0-9).</description>
		/// </item>
		/// </list>
		/// </summary>
		/// <returns>
		/// <code>
		/// True if the password matches the criteria; otherwise, false.
		/// </code>
		/// </returns>

		//https://docs.microsoft.com/pt-br/dotnet/csharp/language-reference/language-specification/documentation-comments#see
		public static bool ValidaSenha(String pPassword)
        {
			if (pPassword.Length != 6)
            {
				return false;
			}
			for (int i = 0; i < 6; i++)
			{
				if (!System.Char.IsDigit(pPassword, i))
					return false;
			}

			return true;
        }

		/// <summary>
		/// Compara a senha digitada com a senha salva.
		/// </summary>
		/// <param name="pSenhaDigitada"></param>
		/// <param name="pSalt"></param>
		/// <param name="pSenhaSalva"></param>
		/// <returns></returns>
		public static bool CompararSenhas(string pSenhaDigitada, string pSalt, byte[] pSenhaSalva)
		{
			if (!Password.CompareByteArrays(Password.GenerateSaltedHash(pSenhaDigitada, pSalt), pSenhaSalva))
			{				
				return false;
			}

			return true;
		}

		/// <summary>Generates Salted Hash code to store passwords safely</summary>		
		public static byte[] GenerateSaltedHash(string pPlainText, string pSalt)
		{
			byte[] plainText = Encoding.UTF8.GetBytes(pPlainText);
			byte[] salt = Encoding.UTF8.GetBytes(pSalt);

			HashAlgorithm algorithm = new SHA256Managed();

			byte[] plainTextWithSaltBytes =
			  new byte[plainText.Length + salt.Length];

			for (int i = 0; i < plainText.Length; i++)
			{
				plainTextWithSaltBytes[i] = plainText[i];
			}
			for (int i = 0; i < salt.Length; i++)
			{
				plainTextWithSaltBytes[plainText.Length + i] = salt[i];
			}

			return algorithm.ComputeHash(plainTextWithSaltBytes);
		}

		/// <summary>Compares two byte arrays</summary>
		public static bool CompareByteArrays(byte[] array1, byte[] array2)
		{
			if (array1.Length != array2.Length)
			{
				return false;
			}

			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>Creates byte salt for hashing</summary>
		public static string CreateSalt(int size)
		{
			//Generate a cryptographic random number.
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] buff = new byte[size];
			rng.GetBytes(buff);

			// Return a Base64 string representation of the random number.
			return Convert.ToBase64String(buff);
		}
	}
}
