using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormatConverter.UnitTest
{
    /// <summary>
    /// FakeClass is used to test the JamlParser against the example.yml file
    /// </summary>
    class FakeClass
    {
        public string teststring;
        public bool testbool;
        public char testchar;
        public float testfloat;

        public FakeSubClass subClass;
        public List<int> intList;
        public List<FakeSubClass> classList;
        public Dictionary<int, List<FakeSubClass>> classMap;
    }

    class FakeSubClass
    {
        public string teststring;
        public int testint;
        public double testdouble;
        public List<string> stringList;

        public FakeSubClass(int test)
        {
            testint = test;
        }

        public FakeSubClass()
        {
        }
    }

    class FakeSubInheritClass : FakeSubClass
    {
        public FakeSubInheritClass(string test)
        {
            base.teststring = test;
        }
    }
}
