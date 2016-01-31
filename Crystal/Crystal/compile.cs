using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal
{
    class compile
    {
        List<token> token = new List<token>();
        DFA sample;
        SyntaxAnalyzer cfg;
        string lexErr = "";
        public compile(string input)
        {
            sample = new DFA();
            Semantic.MainSymbolTable = new globaldata();
            Semantic.ClassSymbolTable = new List<ClassT>();
            Semantic.errorlist = new List<string>();
            List<token> tokenSet = new List<token>();
            tokenSet = sample.LA(input);
            foreach (token s in tokenSet)
            {
                if (s.VP != " " && s.VP != "\n" && s.VP != "\t" && s.VP != "\r")
                {
                    if (s.CP == s.VP)
                    {
                        s.VP = "";
                    }
                    //Console.WriteLine("(" + s.CP + "," + s.VP + "," + s.line + ")");
                    if (s.CP == "Lexical Error")
                    {
                        lexErr += "(" + s.CP + "," + s.VP + "," + s.line + ")\n";
                    }
                    token temp = new token(s.line, s.VP, s.CP);
                    this.token.Add(temp);
                }
            }
            cfg = new SyntaxAnalyzer(this.token);
            cfg.Analyze();
            if (lexErr != "")
            {
                Console.WriteLine(lexErr);
            }
            else
            {
                Console.WriteLine("Lexical Analyzer Success!!");
            }
            Console.WriteLine();
            string synErr = cfg.syntaxError();
            if(synErr != "")
            {
                Console.WriteLine(synErr);
            }
            else
            {
                Console.WriteLine("Syntax Analyzer Success!!");
            }
            Console.WriteLine();
            string SemErr = cfg.semanticError();
            if (SemErr != "")
            {
                Console.WriteLine("Semantic Errors :");
                Console.WriteLine(cfg.semanticError());
            }
            else
            {
                Console.WriteLine("Semantic Analyzer Success!!");
            }
            Console.WriteLine("\n\n\n\n###################### ICG Code ######################");
            Console.WriteLine(cfg.icgCode());
            checkmain();
        }
        public void checkmain()
        {
            bool mainFound = false;
            List<ClassT> Allclasses = Semantic.MainSymbolTable.classes;
            for(int i =0;i<Allclasses.Count;i++)
            {
                for(int j=0;j<Allclasses[i].classdata.Count;j++)
                {
                    if(Allclasses[i].classdata[j].name == "main" && Allclasses[i].classdata[j].methodFlag == true)
                    {
                        mainFound = true;
                    }
                }
            }
            if(!mainFound)
            {
                Console.WriteLine("main function not found");
            }
        }
    }
}
