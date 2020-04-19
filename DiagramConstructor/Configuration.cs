using System;

namespace DiagramConstructor
{
    class Configuration
    {
        public static String defaultFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static String customFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static String finalFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static bool testRun = true;

    }
}
