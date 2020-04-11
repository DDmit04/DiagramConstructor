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

            Console.WriteLine("Parsing code...");
            DiagramBuilder diagramBuider = new DiagramBuilder(codeParser.ParseCode());

            Console.WriteLine("Building diagram...");
            diagramBuider.buildDiagram();
            Console.WriteLine("Finished!");
            Console.WriteLine("Builded diagram was saved to: " + Configuration.finalFilePath);
        }
    }
}
