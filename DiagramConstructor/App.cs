﻿using DiagramConstructor.actor;
using System;
using System.Collections.Generic;

namespace DiagramConstructor
{
    class App
    {
        public void RunApp(String code)
        {

            CodeParser codeParser = new CodeParser(code);
            CodeAnalyzer codeAnalyzer = new CodeAnalyzer();

            Console.WriteLine("\nStaring work");

            Console.WriteLine("Parse code...");
            List<Method> parsedMethods = codeParser.ParseCode();

            Console.WriteLine("Anylize...");
            parsedMethods = codeAnalyzer.analyzeMethods(parsedMethods);

            Console.WriteLine("Build diagram...");
            DiagramBuilder diagramBuider = new DiagramBuilder(parsedMethods);
            diagramBuider.buildDiagram();

            Console.WriteLine("Finished!");
            Console.WriteLine("Builded diagram was saved to: " + Configuration.finalFilePath);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
