using DiagramConstructor.actor;
using DiagramConstructor.Config;
using System;

namespace DiagramConstructor
{

    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("DIAGRAM CONSTRUCTOR 1.0.4.");
            Console.WriteLine("Author: Dmitrochenkov Daniil.\n\n");

            LanguageConfig languageConfig = new CppLanguageConfig();

            CodeParser codeParser = new CodeParser(languageConfig);
            CodeAnalyzer codeAnalyzer = new CodeAnalyzer(languageConfig);

            App app = new App(codeParser, codeAnalyzer, new DiagramBuilder());

            app.RunApp();
        }

    }
}
