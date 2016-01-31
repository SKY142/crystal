using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal
{
    class BreakWord
    {
        public static char[] breakers = { ' ','\r','\t', '\n', '<', '>' , '+', '-', '*', '/', '=', '&', '|', '!', '#', '$', ',', ';', ':', '(', ')',
        '{', '}', '[', ']', '.', '\'', '\"' };
        public List<string> distinctBreaker = new List<string>();
        public List<string> totalBreaker = new List<string>();

        public List<token> breakString(string inputList)
        {
            List<token> output = new List<token>();

            string temp = "";

            bool breaker = false;
            bool addNext = false;
            bool isFloat = false;
            bool isString = false;
            bool newLine = false;
            bool exit = false;
            bool isMultiComment = false;

            int dump = 0;
            int line = 1;

            for (int i = 0; i < inputList.Length; i++)
            {

                //check new line
                if (newLine)
                {
                    line++;
                    newLine = false;
                }

                //--START char ch in breakers
                foreach (char ch in breakers)
                {
                    //--START inputList[i] == ch
                    if (inputList[i] == ch)
                    {
                        breaker = true;
                        //START i != inputList.Length - 1
                        if (i != inputList.Length - 1)
                        {
                            //--START SWITCH--
                            switch (inputList[i])
                            {
                                case ' ':
                                    break;
                                //if \t
                                case '\t':
                                    break;
                                //if next line
                                case '\n':
                                    newLine = true;
                                    break;
                                //IF OPERATORS
                                case '+':
                                    //if not plus minus operator
                                    if (inputList[i + 1] == '+' || inputList[i + 1] == '=')
                                    {
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //means plus minus operator
                                        addNext = false;
                                    }
                                    break;
                                case '-':
                                    //if not plus minus operator
                                    if (inputList[i + 1] == '-' || inputList[i + 1] == '=')
                                    {
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //means plus minus operator
                                        addNext = false;
                                    }
                                    break;
                                case '*':
                                    //if not multiply operator
                                    if (inputList[i + 1] == '=')
                                    {
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //means multiply operator
                                        addNext = false;
                                    }
                                    break;
                                case '/':
                                    //if not divide operator
                                    if (inputList[i + 1] == '=')
                                    {
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //means divide operator
                                        addNext = false;
                                    }
                                    break;
                                case '%':
                                    //if not mode operator
                                    if (inputList[i + 1] == '=')
                                    {
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //means mode operator
                                        addNext = false;
                                    }
                                    break;
                                //END OPERATOR
                                case '>':
                                    //if >= sign
                                    if (inputList[i + 1] == '=')
                                    {
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //if > sign
                                        addNext = false;
                                    }
                                    break;
                                case '<':
                                    if (inputList[i + 1] == '=')
                                    {
                                        //if <= sign
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //if < sign
                                        addNext = false;
                                    }
                                    break;
                                case '=':
                                    if (inputList[i + 1] == '=')
                                    {
                                        //if == sign
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //if = sign
                                        addNext = false;
                                    }
                                    break;
                                case '&':
                                    if (inputList[i + 1] == '&')
                                    {
                                        //if && sign
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //if & sign
                                        addNext = false;
                                    }
                                    break;
                                case '|':
                                    if (inputList[i + 1] == '|')
                                    {
                                        //if || sign
                                        addNext = true;
                                    }
                                    else if (inputList[i + 1] == '#')
                                    {
                                        string tempComment = "|#";
                                        bool commentNOtComplete = true;
                                        isMultiComment = true;
                                        i += 2;
                                        while (i < inputList.Length)
                                        {
                                            tempComment += inputList[i];
                                            if (inputList[i] == '\n')
                                            {
                                                line++;
                                            }

                                            if (i + 1 < inputList.Length && inputList[i] == '#' && inputList[i + 1] == '|')
                                            {
                                                commentNOtComplete = false;
                                                i += 1;
                                                break;
                                            }
                                            else
                                            {
                                                i++;
                                            }
                                        }
                                        if (i < inputList.Length && inputList[i] == '\n')
                                        {
                                            line++;
                                        }

                                        if (commentNOtComplete)
                                        {
                                            output.Add(new token(line, tempComment));
                                        }
                                    }
                                    else
                                    {
                                        //if | sign
                                        addNext = false;
                                    }
                                    break;
                                case '!':
                                    if (inputList[i + 1] == '=')
                                    {
                                        //if != sign
                                        addNext = true;
                                    }
                                    else
                                    {
                                        //if ! sign
                                        addNext = false;
                                    }
                                    break;
                                case '#':
                                    if (inputList[i + 1] == '#')
                                    {
                                        while (true)
                                        {
                                            if (i >= inputList.Length)
                                            {
                                                exit = true;
                                                break;
                                            }
                                            if (inputList[i] == '\n')
                                            {
                                                addNext = false;
                                                newLine = true;
                                                break;
                                            }
                                            i++;
                                        }
                                    }
                                    else
                                    {
                                        //if ! sign
                                        addNext = false;
                                    }
                                    break;
                                case '.':
                                    if (int.TryParse(temp, out dump) && int.TryParse(inputList[i + 1].ToString(), out dump))
                                    {
                                        breaker = false;
                                    }
                                    else if (int.TryParse(inputList[i + 1].ToString(), out dump))
                                    {
                                        isFloat = true;
                                    }
                                    break;
                                case '\"':
                                    if (temp != "")
                                    {
                                        output.Add(new token(line, temp));
                                        temp = "";
                                    }
                                    temp += inputList[i];
                                    i++;
                                    while (i < inputList.Length)// && inputList[i] != '\n'
                                    {
                                        if (inputList[i] == '\\')
                                        {
                                            temp += inputList[i];
                                            if (i + 1 < inputList.Length - 1)
                                            {
                                                temp += inputList[i + 1];
                                            }
                                            i++;

                                        }
                                        else if (inputList[i] == '\"')
                                        {
                                            temp += inputList[i];

                                            break;
                                        }
                                        else
                                        {
                                            temp += inputList[i];
                                        }
                                        i++;
                                    }
                                    if (i < inputList.Length && inputList[i] == '\n')
                                    {
                                        newLine = true;
                                    }
                                    isString = true;
                                    break;
                                case '\'':
                                    //Can be Character
                                    if (temp != "")
                                    {
                                        isString = true;
                                        i--;
                                        break;

                                    }
                                    short length = 0;
                                    while (true)
                                    {

                                        temp += inputList[i];
                                        i++;
                                        if (length >= 2 && inputList[i - 2] != '\\')
                                        {
                                            i--;
                                            isString = true;
                                            break;
                                        }
                                        if (length >= 3 && inputList[i - 2] == '\\' && inputList[i - 3] == '\\')
                                        {
                                            i--;
                                            isString = true;
                                            break;
                                        }
                                        if ((inputList[i] == '\'' && inputList[i - 1] != '\\') || (inputList[i] == '\n') || (i == inputList.Length - 1) || (inputList[i] == '\r' && inputList[i+1]=='\n'))
                                        {
                                            if(!(inputList[i] == '\r' && inputList[i+1]=='\n'))
                                                temp += inputList[i];
                                            isString = true;
                                            if (inputList[i] == '\n')
                                            {
                                                newLine = true;
                                                temp = temp.Remove(temp.Length - 1);
                                            }
                                            break;
                                        }
                                        length++;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            //--END SWITCH--
                        }
                        //--END i != inputList.Length - 1
                    }
                    //--END inputList[i] == ch

                    if (breaker)
                    {
                        break;
                    }
                }
                //--END char ch in breakers

                if (exit)
                {
                    exit = false;
                    break;
                }
                if (!breaker)
                {
                    temp += inputList[i];
                }
                else if (breaker && !isString && !isMultiComment)
                {
                    if (temp != "")
                    {
                        output.Add(new token(line, temp));
                    }
                        temp = inputList[i].ToString();
                    if (isFloat)
                    {
                        breaker = false;
                        isFloat = false;
                        continue;
                    }
                    else if (addNext)
                    {
                        i++;
                        temp += inputList[i];
                        addNext = false;
                    }
                    output.Add(new token(line, temp));
                    temp = "";
                    breaker = false;
                }
                else if (isString)
                {
                    //Breaking String
                    output.Add(new token(line, temp));
                    temp = "";
                    isString = false;
                    breaker = false;
                    if (newLine)
                    {
                        output.Add(new token(line, "\n"));
                    }

                    //Breaking Multicomment

                }
                if (isMultiComment)
                {
                    breaker = false;
                    isMultiComment = false;
                }
            }

            if (temp != "")
            {
                output.Add(new token(line, temp));
                temp = "";
            }

            return output;
        }

    }
}
