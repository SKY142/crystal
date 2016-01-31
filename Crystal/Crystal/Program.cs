using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Crystal
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileToOpen = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\input.txt";
            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = fileToOpen
            };

            process.Start();
            process.WaitForExit();


            //DFA a = new DFA();
            compile com;
            string inputstr = System.IO.File.ReadAllText(fileToOpen);
            try
            {
                //Console.WriteLine();
                com = new compile(inputstr);
                /*foreach (token temp in token)
                {
                    temp.display();
                }*/
                Console.WriteLine();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
