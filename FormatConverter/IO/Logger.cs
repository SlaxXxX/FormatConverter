using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FormatConverter.IO
{
    /// <summary>
    /// Logs information into the console and into a folder, specified in the project settings
    /// </summary>
    static class Logger
    {
        private static string logFolder = FormatConverter.Properties.Settings.Default.LogVerzeichnis;
        private static StreamWriter logFileWriter;
        private static bool logToConsole;
        private static DateTime lastLogged;

        /// <summary>
        /// Creates a new logfile with the current date and time as name
        /// </summary>
        /// <param name="logToConsole"></param>
        public static void create(bool logToConsole)
        {
            Logger.logToConsole = logToConsole;
            lastLogged = DateTime.Now;

            string fileName = logFolder + @"\FormatConverter_" + String.Format("{0:yyyy-MM-dd_hh-mm}", DateTime.Now).ToString() + ".LOG";
            logFileWriter = new StreamWriter(fileName, true);

            logFileWriter.WriteLine(DateTime.Now.ToString() + " :\t\t\t\t" + "<< FormatConverter V." + FormatConverter.Properties.Settings.Default.Version + " >>");
            logFileWriter.WriteLine(DateTime.Now.ToString() + " :\t\t\t\t" + "------- LOG START -------");
            logFileWriter.Flush();
        }

        /// <summary>
        /// Logs a message into the logfile, and console if existent
        /// </summary>
        /// <param name="message">Message</param>
        public static void logMsg(string message)
        {
            logMessage(" :\t\t", message);
        }

        /// <summary>
        /// Logs an errormessage into the logfile, and console if existent
        /// </summary>
        /// <param name="message">Errormessage</param>
        public static void logErr(string message)
        {
            logMessage(" ERROR:\t", message);
        }

        /// <summary>
        /// Logs an errormessage into the logfile, and console if existent
        /// Also logs a stacktrace into the logfile only
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void logErr(string message, Exception exception)
        {
            logMessage(" ERROR:\t", message + "\nSTACKTRACE: " + exception.ToString());
        }

        /// <summary>
        /// Formats messages to log and writes them to specified outputs.
        /// </summary>
        /// <param name="messageType">Type of the message (Contains "ERROR" or nothing)</param>
        /// <param name="message">Content of the message</param>
        private static void logMessage(string messageType, string message)
        {
            DateTime now = DateTime.Now;
            TimeSpan sinceLastLogged = (now - lastLogged);

            if (logToConsole)
                Console.WriteLine(message);

            logFileWriter.WriteLine(now.ToString() + " (" + sinceLastLogged.ToString(@"mm\:ss\:fff") + ")" + messageType + message);

            lastLogged = now;
            logFileWriter.Flush();
        }

        /// <summary>
        /// Closes the filewriter
        /// </summary>
        public static void close()
        {
            logFileWriter.WriteLine(DateTime.Now.ToString() + " :\t\t\t\t" + "-------- LOG END --------");
            logFileWriter.Flush();
            logFileWriter.Close();
        }
    }
}
