using System;
using System.Diagnostics;
using System.IO;
using Visio = Microsoft.Office.Interop.Visio;

namespace DiagramConstructor
{

    class Program
    {

        private static bool test = true;
        static void Main(string[] args)
        {
            String code = "";
            if (test)
            {
                code = "for(awd){for(AWd){if(awd){awd;}else{awd;}awd;}}";
            }
            else
            {
                String answer = "";
                while(answer != "Y" && answer != "N") {
                    Console.WriteLine("open code converter?[Y/N]");
                    answer = Console.ReadLine();
                }
                if(answer == "Y")
                {
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Help\Converter.html");
                }
                Console.WriteLine("input code");
                int bufSize = 2048;
                Stream inStream = Console.OpenStandardInput(bufSize);
                Console.SetIn(new StreamReader(inStream, Console.InputEncoding, false, bufSize));

                code = Console.ReadLine();
            }
            App app = new App();
            app.RunApp(code);
        }
    }

}
