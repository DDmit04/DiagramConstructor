using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramConstructor
{
    class Configuration
    {
        public static String resultFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static String finalFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static bool testRun = false;
    }
}
