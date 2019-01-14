using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using FormatConverter.Parser;
using FormatConverter.IO;
using FormatConverter.Format;

namespace FormatConverter.Converter
{
    /// <summary>
    /// loads configuration files that can be specified as arguments upon launch
    /// </summary>
    class Config
    {
        private List<string> files;

        /// <summary>
        /// Locates config files
        /// </summary>
        /// <returns>Success</returns>
        public bool load()
        {
            string path = @".\config";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            files = Directory.GetFiles(path).ToList();

            if (files.Count > 0)
            {
                string configLog = "Found these configs:";

                foreach (string file in files)
                    configLog += " " + file;
                Logger.logMsg(configLog);
            }
            else
            {
                Logger.logErr("No configs found.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends required config files through the JamlParser
        /// </summary>
        /// <param name="configNames">Required filenames</param>
        /// <returns>Dictionary with Format name mapped to RecordFormat</returns>
        public Dictionary<string, RecordFormat> parse(string[] configNames)
        {
            Dictionary<string, RecordFormat> formats = new Dictionary<string, RecordFormat>();
            foreach (string config in configNames)
            {
                //checks if the configs specified by the user exist
                string file = @".\config\" + config + ".yml";
                if (!files.Contains(file))
                {
                    Logger.logErr("\"" + config + "\" was not found.");
                    continue;
                }
                try
                {
                    if (!formats.ContainsKey(config))
                    {
                        //index needs to be reset after every config, because it's static
                        ColumnInfo.resetIndex();
                        RecordFormat rf = new JamlParser<RecordFormat>().parse(file);
                        rf.name = config;
                        formats.Add(config, rf);
                        Logger.logMsg("Parsed " + config);
                    }
                }
                catch (Exception e)
                {
                    Logger.logErr("Could not parse " + config + ". Skipping.. (See log for further info)", e);
                }
            }

            return formats;
        }
    }
}
