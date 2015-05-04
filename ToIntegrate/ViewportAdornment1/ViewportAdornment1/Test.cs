using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewportAdornment1
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void TestText()
        {
            string text = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\nusing System.Threading.Tasks;\r\n\r\nnamespace ConsoleApplication3\r\n{\r\n    class GrandDad\r\n    {\r\n        public GrandDad()\r\n        {\r\n            GrandDadText = \"GrandDad value\";\r\n        }\r\n\r\n        public string GrandDadText { get; set; }\r\n    }\r\n\r\n    class Dad : GrandDad\r\n    {\r\n        public Dad()\r\n        {\r\n            DadText = \"Dad value\";\r\n        }\r\n\r\n        public string DadText { get; set; }        \r\n    }\r\n\r\n    class Son : Dad\r\n    {\r\n        public Son()\r\n        {\r\n            SonText = \"Son Value\";\r\n        }\r\n\r\n\r\n        public string SonText { get; private set; }\r\n\r\n        public int BarId { get { return 42; } }\r\n    }\r\n\r\n    class Program\r\n    {\r\n        static void Main(string[] args)\r\n        {\r\n            int a = 5 + 7;\r\n\r\n            string[] array = new string[1000];\r\n\r\n            for (int i = 0; i < array.LongLength; i++)\r\n            {\r\n                array[i] = \"Item \" + i;\r\n            }\r\n\r\n\r\n            Son bat = new Son();\r\n\r\n\r\n            Console.WriteLine(array[10]);\r\n            Console.WriteLine(bat);\r\n\r\n\r\n        }\r\n    }\r\n}\r\n";
            int position = 1139;

            Assert.AreEqual("Console.WriteLine", KeywordDetect.DetectKeyword(text, position));
        }

        [Test]
        public void TestText1()
        {
            string text = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\nusing System.Threading.Tasks;\r\n\r\nnamespace ConsoleApplication3\r\n{\r\n    class GrandDad\r\n    {\r\n        public GrandDad()\r\n        {\r\n            GrandDadText = \"GrandDad value\";\r\n        }\r\n\r\n        public string GrandDadText { get; set; }\r\n    }\r\n\r\n    class Dad : GrandDad\r\n    {\r\n        public Dad()\r\n        {\r\n            DadText = \"Dad value\";\r\n        }\r\n\r\n        public string DadText { get; set; }        \r\n    }\r\n\r\n    class Son : Dad\r\n    {\r\n        public Son()\r\n        {\r\n            SonText = \"Son Value\";\r\n        }\r\n\r\n\r\n        public string SonText { get; private set; }\r\n\r\n        public int BarId { get { return 42; } }\r\n    }\r\n\r\n    class Program\r\n    {\r\n        static void Main(string[] args)\r\n        {\r\n            int a = 5 + 7;\r\n\r\n            string[] array = new string[1000];\r\n\r\n            for (int i = 0; i < array.LongLength; i++)\r\n            {\r\n                array[i] = \"Item \" + i;\r\n            }\r\n\r\n\r\n            Son bat = new Son();\r\n\r\n\r\n            Console.WriteLine(array[10]);\r\n            Console.WriteLine(bat);\r\n\r\n\r\n        }\r\n    }\r\n}\r\n";
            int position = 50000;

            Assert.AreEqual("", KeywordDetect.DetectKeyword(text, position));
        }

        [Test]
        public void TestText2()
        {
            string text = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\nusing System.Threading.Tasks;\r\n\r\nnamespace ConsoleApplication3\r\n{\r\n    class GrandDad\r\n    {\r\n        public GrandDad()\r\n        {\r\n            GrandDadText = \"GrandDad value\";\r\n        }\r\n\r\n        public string GrandDadText { get; set; }\r\n    }\r\n\r\n    class Dad : GrandDad\r\n    {\r\n        public Dad()\r\n        {\r\n            DadText = \"Dad value\";\r\n        }\r\n\r\n        public string DadText { get; set; }        \r\n    }\r\n\r\n    class Son : Dad\r\n    {\r\n        public Son()\r\n        {\r\n            SonText = \"Son Value\";\r\n        }\r\n\r\n\r\n        public string SonText { get; private set; }\r\n\r\n        public int BarId { get { return 42; } }\r\n    }\r\n\r\n    class Program\r\n    {\r\n        static void Main(string[] args)\r\n        {\r\n            int a = 5 + 7;\r\n\r\n            string[] array = new string[1000];\r\n\r\n            for (int i = 0; i < array.LongLength; i++)\r\n            {\r\n                array[i] = \"Item \" + i;\r\n            }\r\n\r\n\r\n            Son bat = new Son();\r\n\r\n\r\n            Console.WriteLine(array[10]);\r\n            Console.WriteLine(bat);\r\n\r\n\r\n        }\r\n    }\r\n}\r\n";
            int position = 0;

            Assert.AreEqual("using", KeywordDetect.DetectKeyword(text, position));
        }


    }
}
