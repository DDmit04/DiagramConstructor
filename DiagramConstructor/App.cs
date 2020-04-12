using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramConstructor
{
    class App
    {
        public void RunApp(String code)
        {

            CodeParser codeParser = new CodeParser(code);

            Console.WriteLine("Staring work");

            Console.WriteLine("Parse code...");
            DiagramBuilder diagramBuider = new DiagramBuilder(codeParser.ParseCode());

            Console.WriteLine("Build diagram...");
            diagramBuider.buildDiagram();
            Console.WriteLine("Finished!");
            Console.WriteLine("Builded diagram was saved to: " + Configuration.finalFilePath);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
