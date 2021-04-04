using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIO.Bank
{
    class Logger
    {        
        public Logger() 
        {

        }

        public static void Log(string message)
        {
            try
            {
                //Soluciona problema de diretório variável entre "dotnet run", vscode e visual studio
                String diretorioBase = AppDomain.CurrentDomain.BaseDirectory;
                //https://jeremybytes.blogspot.com/2020/02/set-working-directory-in-visual-studio.html

                //Used for logging
                StringBuilder sb = new StringBuilder();
                DateTime now = DateTime.Now;
                sb.Append(now + " - " + message);
                File.AppendAllText(diretorioBase + "log.txt", sb.ToString());
                sb.Clear();                
            }
            catch (Exception)
            {                
                throw;
            }

            
        }

    }
}
