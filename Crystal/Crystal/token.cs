using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal
{
    class token
    {
        public int line = 1;
        public string VP;
        public string CP;

        public token(int line, string word)
        {
            this.line = line;
            VP = word;
        }
        public token(int line, string word, string classs)
        {
            this.line = line;
            VP = word;
            CP = classs;
        }
        public void display()
        {
            Console.WriteLine("( "+CP+" , "+VP+" , "+line+")\n");
        }
    }
}
/*int opflag = 0;
            int punflag = 0;
            bool dotflag = false;
            bool linechange = false;
            bool lineendflag = false;
            bool mul_com_end = false;
            bool addnext = false;
            for (int i = 0; i < input.Length; i++)
            {
                if(input[i] == '#' && input[i+1] == '#')
                {
                    i = i + 2;
                    while (input[i] != '\r' && input[i + 1] != '\n')
                    {
                        i++;
                        if (i + 1 >= input.Length)
                        {
                            lineendflag = true;
                            break;
                        }
                    }
                    if(lineendflag)
                    {
                        lineendflag = false;
                        break;
                    }
                }
                //multilie comment
                if (input[i] == '|' && input[i + 1] == '#')
                {
                    i = i + 2;
                    while (input[i] != '#' && input[i + 1] != '|')
                    {
                        i++;
                        if (i + 1 >= input.Length)
                        {
                            mul_com_end = true;
                            break;
                        }
                        if (input[i] == '\r' && input[i + 1] == '\n')
                        {
                            line++;
                        }
                    }
                    if (mul_com_end)
                    {
                        mul_com_end = false;
                        break;
                    }
                    i = i + 2;
                }
           
                while (input[i] == ' ')
                    i++;
                while (input[i] == '\t')
                    i++;

                while (i < input.Length && input[i] != ' ')
                {
                    if (input[i] == '"')
                    {
                        temp += input[i++];
                       
                        while (input[i] != '"' && input[i] != '\r' && input[i+1] != '\t')
                        {
                            if (input[i] == '\\' && input[i + 1] == '\"')
                            {
                                temp += input[i];
                                temp += input[++i];
                            }
                            else
                            {
                                temp += input[i];
                            }
                            i++;
                            if(i+1>=input.Length)
                            {
                                break;
                            }
                        }
                        if(input[i] == '"')
                            temp += input[i];
                        break;
                    }
                    if (input[i] == '\'')
                    {
                        int charlength = 0;
                        if (input[i + 1] == '\\')
                        {
                            charlength = 4;
                        }
                        else
                        {
                            charlength = 3;
                        }
                        for (int j = 0; j < charlength; j++, i++)
                        {
                            temp += input[i];
                        }
                        i--;
                        break;
                    }
                    /*if (checkOp(input[i].ToString()) != "")
                    {

                        if (checkOp(input[i].ToString()) != "" && opflag == 0)
                        {
                            opflag++;
                            if (i > 0)
                            {
                                i--;
                                break;
                            }
                        }
                        else if (i+1 < input.Length) 
                        {
                            switch (input[i])
                            {
                                case '+':
                                    if (input[i + 1] == '+' || input[i + 1] == '=')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '-':
                                    if (input[i + 1] == '-' || input[i + 1] == '=')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '*':
                                    if (input[i + 1] == '=')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '/':
                                    if (input[i + 1] == '=')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '%':
                                    if (input[i + 1] == '=')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '>':
                                    if (input[i + 1] == '=')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '<':
                                    if (input[i + 1] == '=')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '=':
                                    if (input[i + 1] == '=')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '!':
                                    if (input[i + 1] == '=')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '&':
                                    if (input[i + 1] == '&')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                                case '|':
                                    if (input[i + 1] == '|')
                                    {
                                        addnext = true;
                                        opflag++;
                                    }
                                    break;
                            }
                        }
                        opflag++;
                    }
                    if ( i + 1 < input.Length && checkOp(input[i].ToString()) != "" )
                    {
                        switch (input[i])
                        {
                            case '+':
                                if (input[i + 1] == '+' || input[i + 1] == '=')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '-':
                                if (input[i + 1] == '-' || input[i + 1] == '=')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '*':
                                if (input[i + 1] == '=')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '/':
                                if (input[i + 1] == '=')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '%':
                                if (input[i + 1] == '=')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '>':
                                if (input[i + 1] == '=')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '<':
                                if (input[i + 1] == '=')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '=':
                                if (input[i + 1] == '=')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '!':
                                if (input[i + 1] == '=')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '&':
                                if (input[i + 1] == '&')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                            case '|':
                                if (input[i + 1] == '|')
                                {
                                    addnext = true;
                                }
                                else
                                    addnext = false;
                                break;
                        }
                    }

                    if (punflag == 0)
                    {
                        if (input[i] != '.')
                            if (checkPunc(input[i].ToString()) != "")
                            {
                                punflag++;
                                i--;
                                break;
                            }
                    }
                    else if (punflag == 1)
                    {
                        punflag++;
                    }

                    if (input[i] == '.')
                    {
                        if (temp != "")
                        {
                            if (checkNum(temp) != "")
                            {
                                dotflag = true;
                                temp += input[i];
                                i++;
                            }
                            else
                            {
                                i--;
                                break;
                            }
                        }
                        if (i + 1 < input.Length && !dotflag)
                        {
                            if (checkNum(input[i + 1].ToString()) == "")
                            {
                                temp += input[i];
                                break;
                            }
                        }
                        else if (dotflag)
                        {
                            if (checkNum(input[i].ToString()) == "")
                            {
                                i--;
                                break;
                            }
                        }
                    }
                    if (input[i] == '\r' && input[i + 1] == '\n')
                    {
                        linechange = true;
                        i++;
                        break;
                    }
                    
                    temp += input[i];
                    if (addnext)
                    {
                        temp += input[++i];
                        break;
                    }
                    else if (checkOp(input[i].ToString()) != "")
                    {
                        break;
                    }

                    else if (opflag == 2)
                    {
                        opflag = 0;
                        //i--;
                        break;
                    }
                    if (punflag > 1)
                    {
                        punflag = 0;
                        break;
                    }

                    //if (opflag > 2)
                    //{
                    //    opflag = 0;
                    //    break;
                    //}

                    i++;
                }
            
                if ((returnStr = checkKW(temp)) != "")
                {
                    if (returnStr == temp)
                        temp_out = "(" + returnStr + "," + "," + line + ")";
                    else
                        temp_out = "(" + returnStr + "," + temp + "," + line + ")";

                }
                else if ((returnStr = checkId(temp)) != "")
                {
                    if (returnStr == temp)
                        temp_out = "(" + returnStr + "," + "," + line + ")";
                    else
                        temp_out = "(" + returnStr + "," + temp + "," + line + ")";

                }
                else if ((returnStr = checkOp(temp)) != "")
                {

                    if (returnStr == temp)
                        temp_out = "(" + returnStr + "," + "," + line + ")";
                    else
                        temp_out = "(" + returnStr + "," + temp + "," + line + ")";

                }
                else if ((returnStr = checkPunc(temp)) != "")
                {
                    if (returnStr == temp)
                        temp_out = "(" + returnStr + "," + "," + line + ")";
                    else
                        temp_out = "(" + returnStr + "," + temp + "," + line + ")";

                }
                else if ((returnStr = checkNum(temp)) != "")
                {
                    if (returnStr == temp)
                        temp_out = "(" + returnStr + "," + "," + line + ")";
                    else
                        temp_out = "(" + returnStr + "," + temp + "," + line + ")";

                }
                else if ((returnStr = checkPoint(temp)) != "" )
                {
                    if (returnStr == temp)
                        temp_out = "(" + returnStr + "," + "," + line + ")";
                    else
                        temp_out = "(" + returnStr + "," + temp + "," + line + ")";

                }
                else if ((returnStr = checkSent(temp)) != "")
                {
                    if (returnStr == temp)
                        temp_out = "(" + returnStr + "," + "," + line + ")";
                    else
                        temp_out = "(" + returnStr + "," + temp + "," + line + ")";

                }
                else if ((returnStr = checkSin(temp)) != "")
                {
                    if (returnStr == temp)
                        temp_out = "(" + returnStr + "," + "," + line + ")";
                    else
                        temp_out = "(" + returnStr + "," + temp + "," + line + ")";
                }
                else
                {
                    returnStr = "Lexical Error";
                    temp_out = "(" + "Lexical Error" + "," + temp + "," + line + ")";
                }
                if (linechange)
                {
                    line++;
                    linechange = false;
                }
                if (temp != "")
                {
                    if (returnStr == temp)
                        temp = "";
                    
                    output.Add(new token(line, temp, returnStr));
                }
                addnext = false;
                temp = "";
                dotflag = false;
               
            }*/
/*private bool THIS()
        {
            //FIRST(<This>)  = {this}
            if (tokenList[index].CP == "this")
            {
                //<this>  this.ID < LISTAOP >
                if (tokenList[index].CP == "this")
                {
                    index++;
                    if (tokenList[index].CP == ".")
                    {
                        index++;
                        if (tokenList[index].CP == "ID")
                        {
                            index++;
                            if (LISTAOP())
                            {
                                return true;
                            }
                        }
                    }
                }

            }

            return false;
        }

        private bool LISTAOP()
        {
            //FIRST(<LISTAOP >) = {; , AOP}

            if (tokenList[index].CP == "AOP" ||
                tokenList[index].CP == ";")
            {
                //<LISTAOP>  ; | AOP < LIST2AOP>
                if (tokenList[index].CP == ";")
                {
                    index++;
                    return true;
                }
                else if (tokenList[index].CP == "AOP")
                {
                    index++;
                    if (LIST2AOP())
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        private bool LIST2AOP()
        {
            //FIRST(<LIST2AOP >) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == Singleton.nonKeywords.INT_CONSTANT.ToString() ||
                tokenList[index].CP == Singleton.nonKeywords.FLOAT_CONSTANT.ToString() ||
                tokenList[index].CP == Singleton.nonKeywords.STRING_CONSTANT.ToString() ||
                tokenList[index].CP == Singleton.nonKeywords.CHAR_CONSTANT.ToString() ||
                tokenList[index].CP == "bool_constant")
            {
                //< LIST2AOP >  ID <INIT> ; | <CONST> ;
                if (CONST())
                {
                    return true;
                }
                else if (tokenList[index].CP == "ID")
                {
                    index++;
                   // if(INIT_Array)
                }
            }
            

            return false;
        }*/