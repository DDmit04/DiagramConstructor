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
                code = "for(a=0){int a = 10;int b -= 10;}";
            }
            else
            {
                Console.WriteLine("input code");
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Help\Converter.html");
                code = Console.ReadLine();
            }
            App app = new App();
            app.RunApp(code);
        }
    }

}
