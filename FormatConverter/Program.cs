using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormatConverter.Converter;
using FormatConverter.Format;
using System.Reflection;
using System.Text.RegularExpressions;
using FormatConverter.IO;
using FormatConverter.Parser;
using FormatConverter.UnitTest;


namespace FormatConverter
{
    /// <summary>
    /// !! If any changes are done to this application, change the version in the application settings !!
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            bool invalidArgs = args.Length < 4;
            Logger.create(invalidArgs);

            //if no arguments specified, open console
            if (invalidArgs)
                FormatConverter.IO.Console.display();

            //uncomment if changes to JamlParser have been made
            //new Unittest();

            if (invalidArgs)
                args = FormatConverter.IO.Console.getArguments();

            Logger.logMsg("-- Starting conversion --");
            Config config = new Config();
            if (config.load())
            {
                //Config files need to be parsed beforehand, so filetypes can be assumed if not given
                Dictionary<string, RecordFormat> formats = config.parse(new[] { args[2], args[3] });
                //if both configs were successfully parsed
                if (formats.ContainsKey(args[2]) && formats.ContainsKey(args[3]))
                {
                    //append filetype if not already specified
                    if (!new Regex(@"\.\w+$").Match(args[0]).Success)
                        args[0] += formats[args[2]].fileType;
                    if (!new Regex(@"\.\w+$").Match(args[1]).Success)
                        args[1] += formats[args[3]].fileType;

                    //file contains input and output path
                    File file = new File(args[0], args[1]);

                    if (file.read())
                    {
                        //fetch, convert and write file content
                        ConvertingProcess converter = new ConvertingProcess(formats[args[2]], formats[args[3]]);
                        string[] converted = converter.convert(file.getContent());
                        file.write(converted, formats[args[3]].fileType);
                    }
                }
                else
                {
                    Logger.logErr("Specified formats are not existent or loaded. Check config.");
                }
            }
            Logger.logMsg("-- Conversion finished --");
            Logger.close();
            if (invalidArgs)
                System.Console.ReadKey();
        }
    }
}
