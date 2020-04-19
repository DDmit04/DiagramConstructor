using System;
using System.Diagnostics;
using System.IO;

namespace DiagramConstructor.utills
{
    class UserInputUtills
    {

        public static void checkConverter()
        {
            if (getYesNoAnsver("open code converter? [Y/N]"))
            {
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Help\Converter.html");
            }
        }

        public static String checkFilepath()
        {
            Console.WriteLine("Iinput result file folder path (press Enter to use default path)");

            String filepath = readFilePath();

            if (filepath.Equals(""))
            {
                filepath = Configuration.defaultFilePath;
                Console.CursorTop--;
            }
            Console.WriteLine("Result file folder path: " + filepath);
            Console.WriteLine("input code to create diagram:");
            return filepath;
        }

        public static String readCode()
        {
            int bufSize = 2048;
            Stream inStream = Console.OpenStandardInput(bufSize);
            Console.SetIn(new StreamReader(inStream, Console.InputEncoding, false, bufSize));

            return Console.ReadLine();
        }

        private static String readFilePath()
        {
            String path = ".";
            do
            {
                if(!path.Equals(".") && !Directory.Exists(path))
                {
                    Console.WriteLine("This derectory isn't exist!");
                }
                path = Console.ReadLine();
            } while (!Directory.Exists(path) && !path.Equals(""));
            return path;
        }

        public static bool getYesNoAnsver(String askMessage)
        {
            bool result = false;
            String answer = "";
            while (answer != "Y" && answer != "N"
                && answer != "y" && answer != "n"
                && answer != "yes" && answer != "no"
                && answer != "Yes" && answer != "No"
                && answer != "YES" && answer != "NO")
            {
                Console.WriteLine(askMessage);
                answer = Console.ReadLine();
            }
            if (answer.Equals("Y") || answer.Equals("y") || answer.Equals("yes") || answer.Equals("Yes") || answer.Equals("YES"))
            {
                result = true;
            } 
            else if(answer.Equals("N") || answer.Equals("n") || answer.Equals("no") || answer.Equals("No") || answer.Equals("NO"))
            {
                result = false;
            }
            return result;
        }

    }
}
