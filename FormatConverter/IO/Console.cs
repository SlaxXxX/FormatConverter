using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FormatConverter.IO
{
    static class Console
    {
        /// <summary>
        /// Opens a console window
        /// </summary>
        public static void display()
        {
            AllocConsole();
        }

        /// <summary>
        /// Writes the string into the consoles output stream
        /// </summary>
        /// <param name="line">String to write</param>
        public static void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        /// <summary>
        /// Takes arguments from the user over the console
        /// </summary>
        /// <returns>Arguments</returns>
        public static string[] getArguments()
        {
            string[] args;

            do
            {
                Logger.logErr("Invalid arguments. Insert <inputfile outputfile inputformat outputformat>:");
                args = System.Console.ReadLine().Split(' ');
            } while (args.Length < 4);

            return args;
        }

        //Copypasta from the internet
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
