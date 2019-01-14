using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FormatConverter.IO;

namespace FormatConverter.Converter
{
    /// <summary>
    /// Stores and handles I/O
    /// </summary>
    class File
    {
        private string inputpath;
        private string outputpath;

        public File(string input, string output)
        {
            inputpath = input;
            outputpath = output;
        }

        /// <summary>
        /// Checks if file exists
        /// </summary>
        /// <returns>If file exists</returns>
        public bool read()
        {
            if (!System.IO.File.Exists(inputpath))
            {
                Logger.logErr("File at " + inputpath + " not found.");
                return false;
            }
            Logger.logMsg("Inputfile found.");
            return true;
        }

        /// <summary>
        /// Returns content of file
        /// </summary>
        /// <returns>Content of file</returns>
        public string[] getContent()
        {
            return System.IO.File.ReadAllLines(inputpath, Encoding.Default);
        }

        /// <summary>
        /// Writes new content to output file
        /// </summary>
        /// <param name="content">New Content</param>
        /// <param name="fileType">Type of the output file. F.ex. ".csv"</param>
        public void write(string[] content, string fileType)
        {
            Logger.logMsg("Writing to outputfile.");
            System.IO.File.WriteAllLines(outputpath, content, Encoding.Default);
        }
    }
}
