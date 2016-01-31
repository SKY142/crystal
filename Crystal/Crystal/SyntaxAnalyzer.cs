using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal
{
    class SyntaxAnalyzer
    {
        List<token> syntaxtoken;
        CFG cfg;
        public SyntaxAnalyzer(List<token> token)
        {
            this.syntaxtoken = token;
            token dollar = new token(-1,"$","$");
            syntaxtoken.Add(dollar);
        }

        public bool Analyze()
        {
            cfg = new CFG(syntaxtoken);
            return cfg.validate();
        }
        public string syntaxError()
        {
            return cfg.syntaxError;
        }
        public string semanticError()
        {
            return cfg.SemanticErrors();
        }
        public string icgCode()
        {
            return cfg.getICGCode();
        }
    }
}
