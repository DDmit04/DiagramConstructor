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
                code = "for(a=0){int b -= 10;a= 10;if(a < 0){int a = 10;}}";
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
                code = Console.ReadLine();
            }
            App app = new App();
            app.RunApp(code);
        }
    }

}
