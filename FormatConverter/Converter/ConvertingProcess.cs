using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormatConverter.Format;
using FormatConverter.IO;

namespace FormatConverter.Converter
{
    /// <summary>
    /// Converts RecordFormat to internal object structure and outputs new RecordFormat
    /// </summary>
    class ConvertingProcess
    {
        private RecordFormat fromFormat;
        private RecordFormat toFormat;

        public ConvertingProcess(RecordFormat from, RecordFormat to)
        {
            fromFormat = from;
            toFormat = to;
        }

        /// <summary>
        /// Converts old RecordFormat to new RecordFormat and applies specified filters
        /// </summary>
        /// <param name="fileContent">Content of the input file</param>
        /// <returns>Content for the output file</returns>
        public string[] convert(string[] fileContent)
        {
            Logger.logMsg("Assigning replacement rules to columns");
            fromFormat.assignReplacements();
            toFormat.assignReplacements();
            Logger.logMsg("Putting data into objects...");
            List<Record> records = fromFormat.formatToRecords(fileContent);
            Logger.logMsg("Filtering objects...");
            records = filterRecords(records);
            Logger.logMsg("Formatting objects into new format...");
            return toFormat.recordsToFileFormat(records, fromFormat);
        }

        /// <summary>
        /// Filters all entries with the criteria specified in both format configs
        /// </summary>
        /// <param name="records">Input list</param>
        /// <returns>Filtered List</returns>
        private List<Record> filterRecords(List<Record> records)
        {
            List<Record> fromFiltered = new List<Record>();
            if (fromFormat.filter != null)
            {
                foreach (Record record in records)
                {
                    bool filtered = false;
                    foreach (KeyValuePair<string, List<string>> filterPair in fromFormat.filter.ToList<KeyValuePair<string, List<string>>>())
                        if (filtered || !filterPair.Value.Contains(record.getData()[fromFormat.format[filterPair.Key].index]))
                            filtered = true;
                    if (!filtered)
                        fromFiltered.Add(record);
                }
            }
            else
                fromFiltered = records;

            List<Record> toFiltered = new List<Record>();
            if (toFormat.filter != null)
            {
                foreach (Record record in fromFiltered)
                {
                    bool filtered = false;
                    foreach (KeyValuePair<string, List<string>> filterPair in toFormat.filter.ToList<KeyValuePair<string, List<string>>>())
                        if (filtered || !filterPair.Value.Contains(record.getData()[fromFormat.format[filterPair.Key].index]))
                            filtered = true;
                    if (!filtered)
                        toFiltered.Add(record);
                }
            }
            else
                toFiltered = fromFiltered;

            return toFiltered;
        }

    }
}
