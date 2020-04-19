using DiagramConstructor.actor;
using DiagramConstructor.utills;
using System;
using System.Collections.Generic;

namespace DiagramConstructor
{
    class App
    {

        private CodeParser codeParser;
        private CodeAnalyzer codeAnalyzer;
        private DiagramBuilder diagramBuider;
        String code = "";

        public App(CodeParser codeParser, CodeAnalyzer codeAnalyzer, DiagramBuilder diagramBuider)
        {
            this.codeParser = codeParser;
            this.codeAnalyzer = codeAnalyzer;
            this.diagramBuider = diagramBuider;
        }

        public void RunApp()
        {
            do
            {
                code = getUserInputs();

                Console.WriteLine("\nStar work");

                Console.WriteLine("Parse code...");
                List<Method> parsedMethods = codeParser.ParseCode(code);

                Console.WriteLine("Anylize...");
                parsedMethods = codeAnalyzer.analyzeMethods(parsedMethods);

                Console.WriteLine("Build diagram...");
                diagramBuider.buildDiagram(parsedMethods);

                Console.WriteLine("Finished!");
                Console.WriteLine("Builded diagram was saved to: " + Configuration.finalFilePath + "\n");
            } while (!UserInputUtills.getYesNoAnsver("Exit program? [Y/N]"));
        }

        public static String getUserInputs()
        {
            String code = "";
            if (Configuration.testRun)
            {
                code = "main(){while(adw){awdawd;dawawd;while(adwz){awd;awd;awd;}awd;}awd;}";
            }
            else
            {
                UserInputUtills.checkConverter();
                Configuration.customFilePath = UserInputUtills.checkFilepath();
                code = UserInputUtills.readCode();
            }
            return code;
        }

    }
}
