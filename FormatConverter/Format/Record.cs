using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormatConverter.Format
{
    /// <summary>
    /// Contains one row of data
    /// </summary>
    class Record
    {
        private List<string> data;

        public Record(List<string> data)
        {
            this.data = data;
        }

        public List<string> getData()
        {
            return data;
        }
    }
}
