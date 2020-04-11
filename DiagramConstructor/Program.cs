using System;
using System.Diagnostics;
using System.IO;

namespace DiagramConstructor
{

    class Program
    {

        static void Main(string[] args)
        {
            String code = "";
            if (Configuration.testRun)
            {
                code = "voidprintArray(intnums[],intlen){for(inti=0;i<len;i++){cout<<nums[i]<<'';}cout<<endl;}intfindMinIndex(intarr[],intarrLength,intstart){intminIndex=start;intminElem=arr[start];for(inti=start;i<arrLength;i++){if(arr[i]<minElem){minElem=arr[i];minIndex=i;}}returnminIndex;}intmain(){inthelpVar=0;intminIndex=0;intarrLen=5;cout<<'inputarrlength\n';cin>>arrLen;int*nums=newint[arrLen];for(inti=0;i<arrLen;i++){cout<<'input['<<i<<']elementofarray\n';cin>>nums[i];}cout<<'inputedarray:\n';printArray(nums,arrLen);for(inti=0;i<arrLen;i++){minIndex=findMinIndex(nums,arrLen,i);helpVar=nums[i];nums[i]=nums[minIndex];nums[minIndex]=helpVar;}cout<<'3maxelements:\n';cout<<endl<<nums[arrLen-1]<<','<<nums[arrLen-2]<<','<<nums[arrLen-3]<<endl;}";
            }
            else
            {
                if(askOpenConverter().Equals("Y"))
                {
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Help\Converter.html");
                }
                Console.WriteLine("Iinput result file folder path (press Enter to use default path)");

                String filepath = readFilePath();
                if (!filepath.Equals(""))
                {
                    Configuration.customFilePath = filepath;
                }
                Console.WriteLine("Result file folder path: " + Configuration.customFilePath);
                Console.WriteLine("input code to create diagram:");
                code = readCode();
            }
            App app = new App();
            app.RunApp(code);
        }

        public static String readFilePath()
        {
            String path = ".";
            do
            {
                path = Console.ReadLine();
            } while (!Directory.Exists(path) && !path.Equals(""));
            return path;
        }

        public static String readCode()
        {
            int bufSize = 2048;
            Stream inStream = Console.OpenStandardInput(bufSize);
            Console.SetIn(new StreamReader(inStream, Console.InputEncoding, false, bufSize));

            return Console.ReadLine();
        }

        public static String askOpenConverter()
        {
            String answer = "";
            while (answer != "Y" && answer != "N")
            {
                Console.WriteLine("open code converter?[Y/N]");
                answer = Console.ReadLine();
            }
            return answer;
        }

    }
}
