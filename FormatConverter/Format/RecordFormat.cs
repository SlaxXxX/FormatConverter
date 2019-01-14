using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using FormatConverter.Format;

namespace FormatConverter.Format
{
    /// <summary>
    /// This is the class that gets constructed by the JamlParser from a config file
    /// All fields that have to be filled by the parser have to be public
    /// </summary>
    class RecordFormat
    {
        public string name;
        public IFormatter formatter;
        public Dictionary<string, ColumnInfo> format;
        public Dictionary<string, ReplacementRule> replace;
        public Dictionary<string, List<string>> filter;
        public string dataSeparator;
        public string fileType;
        public string notExistent;
        public bool makeHeader;

        /// <summary>
        /// Calls this format's formatter to turn the input file into a list of records
        /// </summary>
        /// <param name="formattedContent">Content of the input file</param>
        /// <returns>List or records</returns>
        public List<Record> formatToRecords(string[] formattedContent)
        {
            return formatter.getElements(formattedContent, this);
        }

        /// <summary>
        /// Calls this format's formatter to turn the list of records into the output file
        /// </summary>
        /// <param name="records">List of records</param>
        /// <param name="before">The format of the input file</param>
        /// <returns>Content of the output file</returns>
        public string[] recordsToFileFormat(List<Record> records, RecordFormat before)
        {
            return formatter.formatElements(records, this, before);
        }

        public void assignReplacements()
        {
            if (replace != null)
                foreach (KeyValuePair<string, ReplacementRule> pair in replace)
                {
                    if (pair.Value.on != null)
                    {
                        foreach (string column in pair.Value.on)
                        {
                            format[column].addReplacement(pair.Key,pair.Value.to);
                        }
                    }
                    else
                    {
                        foreach (ColumnInfo column in format.Values)
                        {
                            column.addReplacement(pair.Key, pair.Value.to);
                        }
                    }
                }
        }
    }

    /// <summary>
    /// Defines what characters need to be replaced on which Columns
    /// </summary>
    class ReplacementRule
    {
        public string to;
        public List<string> on;
    }

    /// <summary>
    /// Contains information about a column of data
    /// </summary>
    class ColumnInfo
    {
        private static int currentIndex = 0;
        public int index;
        public int start;
        public int length;
        public string defaultValue;
        public Dictionary<string, string> replacements = new Dictionary<string,string>();

        public bool hasDefault()
        {
            return defaultValue != null;
        }

        public static void resetIndex()
        {
            currentIndex = 0;
        }

        public ColumnInfo()
        {
            index = currentIndex;
            currentIndex++;
        }

        public ColumnInfo(string defaultValue) : this()
        {
            this.defaultValue = defaultValue;
        }

        public ColumnInfo(int start, int length)
        {
            index = currentIndex;
            currentIndex++;
            this.start = start - 1;
            this.length = length;
        }

        public ColumnInfo(int start, int length, string defaultValue) : this(start, length)
        {
            this.defaultValue = defaultValue;
        }

        public void addReplacement(string from, string to)
        {
            replacements.Add(from, to);
        }

        public Dictionary<string, string> getReplacements()
        {
            return replacements;
        }
    }
}
