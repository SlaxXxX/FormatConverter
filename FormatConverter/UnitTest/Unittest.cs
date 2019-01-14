using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormatConverter.Parser;
using FormatConverter.IO;

namespace FormatConverter.UnitTest
{
    /// <summary>
    /// Tests functionality of the JamlParser. This crude test only checks if any exceptions occur,
    /// it does not validate results or test invalid inputs
    /// </summary>
    class Unittest
    {
        public Unittest()
        {
            Logger.logMsg("Starting unittest.");
            JamlParser<FakeClass> parser = new JamlParser<FakeClass>();
            FakeClass fake = parser.parse("config\\example.yml");
            Logger.logMsg("Unittest finished. Everything seems to be working.");
        }
    }
}
