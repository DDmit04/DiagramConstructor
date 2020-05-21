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
            if(Configuration.testRun == TestRunType.SINGLE_TEST)
            {
                code = "main(){if(a>10){do{a=10;b=20;cout<<'lul'<<a;}while(b<100);}}";
            }
            else if (Configuration.testRun == TestRunType.FULL_TEST)
            {
                code = "forForTest(){for(i=0;i<10;i++){for(i=0;i<10;i++){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput'<<cout<<ded;}}}whileForTest(){while((a>10||v<10)&&s==10){for(i=0;i<10;i++){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}}forWhileTest(){for(i=0;i<10;i++){while(a>10){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}}whileIfTest(){while(a>10){if((a>10||v<10)&&s==10){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}else{s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}}whileElseIfTest(){while(a>10){if(a>10){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}elseif(a<10){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}elseif(a>10){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}else{s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}}forIfTest(){for(i=0;i<10;i++){if(){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}else{s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}}forElseIfTest(){for(i=0;i<10;i++){if(a>10){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}elseif(a<10){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}elseif(a>10){s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}else{s=10;actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}}iftest(){if(actor<0){s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}ifElsetest(){if(actor<0){s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}else{s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}elseifTest(){if(actor<0){s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}elseif(actor<10){s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}elseif(actor<10){s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}else{s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}forTest(){for(i=0;i<10;i++){s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}whileTest(){while(actor>0){s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}}doWhileTest(){do{s=10;doSome();actor=10;cout<<'someoutput'<<Count;cout<<'someoutput';}while(sbyte>10);}";
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
