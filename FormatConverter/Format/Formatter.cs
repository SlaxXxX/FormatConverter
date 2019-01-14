using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormatConverter.Format;
using System.Text.RegularExpressions;

namespace FormatConverter.Format
{
    /// <summary>
    /// The formatter contains the logic for converting from- and to file format, using the information stored in RecordFormat
    /// futher implementations of the Formatter can be added, if needed
    /// </summary>
    interface IFormatter
    {
        /// <summary>
        /// Turns content of input file into list of records with specified rules from the record format
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        List<Record> getElements(string[] elements, RecordFormat format);

        /// <summary>
        /// Formats list of records into the output format with specified rules from the record format
        /// </summary>
        /// <param name="records"></param>
        /// <param name="after"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        string[] formatElements(List<Record> records, RecordFormat after, RecordFormat before);
    }

    class Relative : IFormatter
    {
        public List<Record> getElements(string[] elements, RecordFormat format)
        {
            List<Record> records = new List<Record>();
            for (int i = format.makeHeader ? 1 : 0; i < elements.Length; i++)
            {
                string[] row = elements[i].Split(format.dataSeparator.ToCharArray());
                row = Util.replaceAll(row, format.format.Values);
                records.Add(new Record(new List<string>(row)));
            }

            return records;
        }

        public string[] formatElements(List<Record> records, RecordFormat after, RecordFormat before)
        {
            int headerOffset = after.makeHeader ? 1 : 0;
            string[] formattedElements = new string[records.Count + headerOffset];

            if (after.makeHeader)
            {
                string header = "";
                string[] data = after.format.Keys.ToArray();
                for (int i = 0; i < data.Length; i++)
                {
                    header += data[i];
                    if (i < data.Length - 1)
                        header += after.dataSeparator;
                }
                formattedElements[0] = header;
            }

            foreach (KeyValuePair<string, ColumnInfo> pair in after.format.ToList<KeyValuePair<string, ColumnInfo>>())
            {
                for (int i = 0; i < records.Count; i++)
                {
                    string data;
                    bool contained = before.format.ContainsKey(pair.Key);

                    data = contained ? records[i].getData()[before.format[pair.Key].index] : after.notExistent;
                    data = data.Equals(before.notExistent) ? after.notExistent : data;
                    data = pair.Value.hasDefault() && data.Equals(after.notExistent) ? pair.Value.defaultValue : data;
                    data = Util.replace(data, pair.Value);
                    if (pair.Value.index < after.format.Count - 1)
                        data += after.dataSeparator;
                    formattedElements[i + headerOffset] += data;
                }
            }
            return formattedElements;
        }
    }

    class Absolute : IFormatter
    {
        public List<Record> getElements(string[] elements, RecordFormat format)
        {
            List<Record> records = new List<Record>();

            for (int i = format.makeHeader ? 1 : 0; i < elements.Length; i++)
            {
                List<string> data = new List<string>();
                foreach (KeyValuePair<string, ColumnInfo> pair in format.format.ToList<KeyValuePair<string, ColumnInfo>>())
                {
                    data.Add(getElement(elements[i], format.dataSeparator, pair.Value));
                }
                data = Util.replaceAll(data, format.format.Values);
                records.Add(new Record(data));
            }

            return records;
        }

        private string getElement(string element, string dataSeparator, ColumnInfo info)
        {
            string substring = element.Substring(info.start, info.length);

            int blankSpace;
            for (blankSpace = substring.Length;
                blankSpace > dataSeparator.Length - 1 && substring.Substring(blankSpace - dataSeparator.Length, dataSeparator.Length).Equals(dataSeparator);
                blankSpace -= dataSeparator.Length) { }
            return substring.Substring(0, blankSpace);
        }

        public string[] formatElements(List<Record> records, RecordFormat after, RecordFormat before)
        {
            int headerOffset = after.makeHeader ? 1 : 0;
            string[] formattedElements = new string[records.Count + headerOffset];
            for (int i = 0; i < formattedElements.Length; i++)
                formattedElements[i] = "";

            if (after.makeHeader)
            {
                string header = "";
                foreach (KeyValuePair<string, ColumnInfo> pair in after.format.ToList())
                {
                    if (pair.Value.start < header.Length)
                        continue;
                    header += fillBlank(after.dataSeparator, pair.Value.start - header.Length);
                    header += putToLength(after.dataSeparator, pair.Key, pair.Value.length);
                }
                formattedElements[0] = header;
            }

            int index = 0;
            foreach (KeyValuePair<string, ColumnInfo> pair in after.format)
            {
                bool contained = before.format.ContainsKey(pair.Key);
                if (pair.Value.start < index)
                    continue;
                for (int i = 0; i < records.Count; i++)
                {
                    formattedElements[i + headerOffset] += fillBlank(after.dataSeparator, pair.Value.start - formattedElements[i + headerOffset].Length);

                    string data = contained ? records[i].getData()[before.format[pair.Key].index] : after.notExistent;
                    data = data.Equals(before.notExistent) ? after.notExistent : data;
                    data = pair.Value.hasDefault() && data.Equals(after.notExistent) ? pair.Value.defaultValue : data;
                    data = Util.replace(data, pair.Value);

                    formattedElements[i + headerOffset] += putToLength(after.dataSeparator, data, pair.Value.length);
                    index = pair.Value.start + pair.Value.length;
                }
            }

            return formattedElements;
        }

        private string putToLength(string dataSeparator, string str, int length)
        {
            if (str.Length > length)
                return str.Substring(0, length);
            else
                return str + fillBlank(dataSeparator, length - str.Length);
        }

        private string fillBlank(string blankSymbol, int width)
        {
            string blank = "";

            for (int i = 0; i < width; i += blankSymbol.Length)
                blank += blankSymbol;

            return blank;
        }
    }

    class Util
    {
        public static List<string> replaceAll(List<string> data, Dictionary<string, ColumnInfo>.ValueCollection collection)
        {
            for (int i = 0; i < data.Count; i++)
            {
                ColumnInfo info = collection.Where(value => value.index == i).First();
                data[i] = replace(data[i], info);
            }
            return data;
        }

        public static string[] replaceAll(string[] data, Dictionary<string, ColumnInfo>.ValueCollection collection)
        {
            for (int i = 0; i < data.Length; i++)
            {
                ColumnInfo info = collection.Where(value => value.index == i).First();
                data[i] = replace(data[i], info);
            }
            return data;
        }

        public static string replace(string data, ColumnInfo info)
        {
            foreach (KeyValuePair<string, string> replacement in info.replacements)
            {
                data = Regex.Replace(data, replacement.Key, replacement.Value);
            }
            return data;
        }
    }
}
