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
                code = "filename = 'tabl_umn.txt';main(){ifstreamreadFileStream(filename);buffer = '';if(readFileStream.is_open()){cout<<'fileisexists!''';while(getline(readFileStream,buffer)){cout<<buffer<<''';}}else{ofstreamwriteFileStream(filename);cout<<'fileisn'texists!'';for(i = 2;i<10;i++){for(j = 2;j<10;j++){writeFileStream<<i<<'X'<<j<<' = '<<i*j<<''';}writeFileStream<<''';}cout<<'filecreated!';}}";
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
