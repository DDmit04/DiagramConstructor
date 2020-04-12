using System;
using System.Diagnostics;
using System.IO;

namespace DiagramConstructor
{

    class Program
    {

        static void Main(string[] args)
        {
            String code = "";
            if (Configuration.testRun)
            {
                code = "main(){if(111){if(222){111;}else{222;}}}";
            }
            else
            {
                if(askOpenConverter().Equals("Y"))
                {
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Help\Converter.html");
                }
                Console.WriteLine("Iinput result file folder path (press Enter to use default path)");

                String filepath = readFilePath();
                if (!filepath.Equals(""))
                {
                    Configuration.customFilePath = filepath;
                }
                Console.WriteLine("Result file folder path: " + Configuration.customFilePath);
                Console.WriteLine("input code to create diagram:");
                code = readCode();
            }
            App app = new App();
            app.RunApp(code);
        }

        public static String readFilePath()
        {
            String path = ".";
            do
            {
                path = Console.ReadLine();
            } while (!Directory.Exists(path) && !path.Equals(""));
            return path;
        }

        public static String readCode()
        {
            int bufSize = 2048;
            Stream inStream = Console.OpenStandardInput(bufSize);
            Console.SetIn(new StreamReader(inStream, Console.InputEncoding, false, bufSize));

            return Console.ReadLine();
        }

        public static String askOpenConverter()
        {
            String answer = "";
            while (answer != "Y" && answer != "N")
            {
                Console.WriteLine("open code converter?[Y/N]");
                answer = Console.ReadLine();
            }
            return answer;
        }

    }
}
