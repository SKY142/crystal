using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal
{
    class ICG
    {
        int label = 1;
        int tempVar = 1;
        public string IcgOutput = "";
        public string CreateLabel()
        {
            return "L" + label++;
        }

        public string CreateTemp()
        {
            return "T" + tempVar++;
        }

        public string getLabel()
        {
            return "L" + (label - 1);
        }
        public string getTemp()
        {
            return "T" + (tempVar - 1);
        }

        public void gen(string input)
        {
            IcgOutput += input + "\n";
        }
        public string getcode()
        {
            return IcgOutput;
        }
    }
}
