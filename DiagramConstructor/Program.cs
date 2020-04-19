using DiagramConstructor.actor;
using System;

namespace DiagramConstructor
{

    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("DIAGRAM CONSTRUCTOR 1.0.2.");
            Console.WriteLine("Author: Dmitrochenkov Daniil.\n\n");

            App app = new App(new CodeParser(), new CodeAnalyzer(), new DiagramBuilder());
            app.RunApp();
        }

    }
}
