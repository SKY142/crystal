using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal
{
    class CFG
    {
        ICG icg = new ICG();
        int index = 0;
        List<token> tokenList;

        public CFG(List<token> token)
        {
            this.tokenList = token;
        }

        public string getICGCode()
        {
            return icg.getcode();
        }

        //semanticCode
        Semantic semanticAnalyzer = new Semantic();
        private string getType(string name, string err)
        {
            string Type = "";
            if (semanticAnalyzer.Search(name))
            {
                Type = semanticAnalyzer.getType(name);
                if (Type == "invalid")
                {
                    addError(err);
                    return "invalid";
                }
                return Type;
            }
            else
            {
                addError(err);
                return "invalid";
            }
        }
        private bool inMethod = false;
        private void addError(string e)
        {
            Semantic.errorlist.Add(" "+ e + " " + tokenList[index].VP + " at line "+tokenList[index].line);
        }
        public string syntaxError = "";
        public string SemanticErrors()
        {
            string err = "";
            foreach (string e in Semantic.errorlist)
            {
                err += e + "\n";
                //Console.WriteLine(e+"\n");
            }
            return err;
        }

        //validate
        public bool validate()
        {
            if (class_dec())
            {
                if (tokenList[index].CP == "$")
                {
                    return true;
                }
            }
            syntaxError = "Syntax Error at line " + tokenList[index].line;
            return false;
        }

        private bool class_dec()
        {
            string AM = "";
            if (tokenList[index].CP == "Acess_Modifier" ||
                tokenList[index].CP == "class")
            {


                if (Access_Mod(ref AM))
                {
                    if (Class_Dec1(AM))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool Access_Mod(ref string AM)
        {
            //first
            if (tokenList[index].CP == "Acess_Modifier")
            {
                if (tokenList[index].CP == "Acess_Modifier")
                {
                    AM = tokenList[index].VP;
                    index++;
                    return true;
                }
            }

            //follow(<Access_Modifier>)
            else if (tokenList[index].CP == "class" ||
                tokenList[index].CP == "static" ||
                tokenList[index].CP == "DT" ||
                tokenList[index].CP == "void" ||
                tokenList[index].CP == "ID")
                {
                    return true;
                }

            return false;
        }

        private bool Class_Dec1(string AM)
        {
            string N = "";
            string PN = "";
            //FIRST
            if (tokenList[index].CP == "class")
            {
                index++;
                if (tokenList[index].CP == "ID")
                {
                    N = tokenList[index].VP;
                    index++;
                    if (Inherit_Class(ref PN))
                    {
                        semanticAnalyzer.insertClass(N, AM, PN);
                        if (tokenList[index].CP == "{")
                        {
                            semanticAnalyzer.createScope();
                            index++;
                            if (Class_Body())
                            {
                                if (tokenList[index].CP == "}")
                                {
                                    semanticAnalyzer.deleteScope();
                                    index++;
                                    if (Multi_Class())
                                    {
                                        return true;
                                    }   
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool Multi_Class()
        {
            if (tokenList[index].CP == "Acess_Modifier" ||
                tokenList[index].CP == "class" ||
                tokenList[index].CP == "$")
            {
                if (class_dec())
                {
                    return true;
                }
                else if (tokenList[index].CP == "$")
                {
                    return true;
                }
            }
            return false;
        }

        private bool Inherit_Class(ref string PN)
        {
            //FIRST
            if (tokenList[index].CP == ":")
            {
                
                index++;
                if (tokenList[index].CP == "ID")
                {
                    PN = tokenList[index].VP;
                    if (!(semanticAnalyzer.ClassLookUp(PN)))
                    {
                        addError("Undeclared Class");
                    }
                    index++;
                    return true;
                }
            }

            //FOLLOW(<Class_Base>)
            else if (tokenList[index].CP == "{")
            {
                return true;
            }

            return false;
        }

        private bool Class_Body()
        {
            //FIRST
            if (tokenList[index].CP == "Acess_Modifier" ||
                tokenList[index].CP == "static" ||
                tokenList[index].CP == "DT" ||
                tokenList[index].CP == "void" ||
                tokenList[index].CP == "ID" ||
                tokenList[index].CP == "class")
            {
                
                if (Class_Member())
                {
                    if (Class_Body())
                    {
                        return true;
                    }
                }
            }

            //FOLLOW(<Class_Body>)
            else if (tokenList[index].CP == "}")
            {
                return true;
            }

            return false;
        }

        private bool Class_Member()
        {
            //FIRST
            if (tokenList[index].CP == "Acess_Modifier" ||
                tokenList[index].CP == "static" ||
                tokenList[index].CP == "DT" ||
                tokenList[index].CP == "void" ||
                tokenList[index].CP == "ID" ||
                tokenList[index].CP == "class")
            {
                string AM = "";
                if (Access_Mod(ref AM))
                {
                    if (Class_Members1(AM))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool Class_Members1(string AM)
        {
            //FIRST
            if (tokenList[index].CP == "static" ||
                tokenList[index].CP == "DT" ||
                tokenList[index].CP == "void" ||
                tokenList[index].CP == "ID" ||
                tokenList[index].CP == "class")
            {
                string TM = "";
                if (tokenList[index].CP == "static")
                {
                    TM = tokenList[index].CP;
                    index++;
                    if (static_members(AM, TM))
                    {
                        return true;
                    }
                }

                else if (tokenList[index].CP == "void")
                {
                    string RT = tokenList[index].CP;
                    index++;
                    if (tokenList[index].CP == "ID")
                    {
                        string N = tokenList[index].VP;
                        index++;
                        if (Method_Link(AM,TM,RT,N))
                        {
                            return true;
                        }
                    }
                }

                else if (tokenList[index].CP == "DT")
                {
                    string DT = tokenList[index].VP;
                    index++;
                    if (Dec_2(AM,DT))
                    {
                        return true;
                    }
                }

                else if (tokenList[index].CP == "ID")
                {
                    string N = tokenList[index].VP;
                    index++;
                    if (!semanticAnalyzer.ClassLookUp(N))
                    {
                        addError("Class does not exists");
                    }
                    if (Object_Constructor_Dec(AM, N))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Object_Constructor_Dec(string AM, string N)
        {
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "[" ||
                tokenList[index].CP == "(")
            {
                
                if (Object_link(AM, N))
                {
                    return true;
                }
                else if (Constructor_DEC(AM, N))
                {
                    return true;
                }
            }
            return false;
        }

        private bool Dec_2(string T,string AM)
        {
            //FIRST(<DT_2>)
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "[")
            {
                if (tokenList[index].CP == "ID")
                {
                    string N = tokenList[index].VP;

                    index++;
                    if (ID_1(AM,T,N))
                    {
                        return true;
                    }
                }

                else if (Array_DEC(AM,"",T))
                {
                    return true;
                }
            }
            return false;
        }

        private bool Constructor_DEC(string AM,string N)
        {
            //FIRST(<Constructor_DEC>)
            if (tokenList[index].CP == "(")
            {
                string PL = "",NPL ="";
                string AL = "", NAL = "";
                if (tokenList[index].CP == "(")
                {
                    index++;
                    semanticAnalyzer.createScope();
                    if (List_Param_Dec(ref AL, PL, ref NAL, NPL))
                    {
                        if (tokenList[index].CP == ")")
                        {
                            index++;
                            string T = AL + "> Null"; 
                            MethodT cons = new MethodT(N, T, AM, "", true);
                            semanticAnalyzer.insertConstructor(cons);
                            if (AL != "")
                            {
                                string[] arr = AL.Split(',');
                                string[] Narr = NAL.Split(',');
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    semanticAnalyzer.insertVariables(Narr[i], arr[i], semanticAnalyzer.getCurrentScope());
                                }

                            }
                            if (tokenList[index].CP == "{")
                            {
                                index++;
                                
                                string T1 = "";
                                if (M_ST(ref T1))
                                {
                                    if (tokenList[index].CP == "}")
                                    {
                                        index++;
                                        semanticAnalyzer.deleteScope();
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        
        private bool ID_1(string AM, string RT, string N)
        {
            //FIRST(<ID_1>) = {( , AOP , , , ; }
            if (tokenList[index].CP == "(" ||
                tokenList[index].CP == "AOP" ||
                tokenList[index].CP == "," ||
                tokenList[index].CP == ";")
            {
                //<ID_1><Varaiable_Link2> | <Method_Link 3>
                if (Variable_Link2(RT,N))
                {
                    return true;
                }
                else if (Method_Link(AM, "", RT, N))
                {
                    return true;
                }
            }
            return false;
        }

        private bool static_members(string AM, string TM)
        {


            if (tokenList[index].CP == "DT" ||
                tokenList[index].CP == "ID" ||
                tokenList[index].CP == "void")
            {
                if (tokenList[index].CP == "DT")
                {
                    string RT = tokenList[index].VP;
                    index++;
                    if (Dec_1(AM, TM, RT))
                    {
                         return true;
                    }
                }

                else if (tokenList[index].CP == "void")
                {
                    string RT = tokenList[index].VP;
                    index++;
                    if (tokenList[index].CP == "ID")
                    {
                        string N = tokenList[index].VP;
                        index++;
                        if (Method_Link(AM, TM, RT, N))
                        {
                           return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool Dec_1(string AM,string TM,string RT)
        {
            //FIRST(<DT_1>) = {ID , [}
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "[")
            {
                //<DT_1>  ID <ID_2>| <Array_DEC>
                if (tokenList[index].CP == "ID")
                {
                    string N = tokenList[index].VP;
                    index++;
                    if (ID_2(AM,TM,RT,N))
                    {
                        return true;
                    }
                }
                else if (Array_DEC(AM, TM, RT))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ID_2(string AM,string TM,string RT,string N)
        {
            //FIRST(<ID_2>) = {( , =}
            if (tokenList[index].CP == "(" ||
                tokenList[index].CP == "AOP")
            {
                //<ID_2>  <Method_Link3> | <Variable_Link2>
                if (Method_Link(AM, TM, RT, N))
                {
                    return true;
                }
                else if (Variable_Link2(RT,N))
                {
                    return true;
                }
            }
            return false;
        }

        //private bool Id_OArray()
        //{
        //    //FIRST(<Id_OArray>) = {ID , [}
        //    if (tokenList[index].CP == "ID" ||
        //        tokenList[index].CP == "[")
        //    {
        //        //<Id_OArray>  ID <Id_A> | <Array_DEC>
        //        if (tokenList[index].CP == "ID")
        //        {
        //            index++;
        //            if (Id_A())
        //            {
        //                return true;
        //            }
        //        }
        //        else if (Array_DEC())
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

      /*  private bool Id_A()
        {
            //FIRST(<Id_A>) = {= , (}
            if (tokenList[index].CP == "AOP" ||
                tokenList[index].CP == "(")
            {
                //<Id_A>  <Method_Link3> | <Object_Creation_Exp>
                if (Method_Link3())
                {
                    return true;
                }
                else if (Object_Creation_Exp())
                {
                    return true;
                }
            }

            return false;
        }
        */
        private bool Exp(ref string T,ref string Nexp)
        {
            //FIRST(<Exp>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST  }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant"||
                tokenList[index].CP == "bool_constant"
            )
            {
                //<Exp>  <OR_Exp>
                if (OR_Exp(ref T,ref Nexp))
                {
                    return true;
                }
            }
            return false;
        }
        private bool OR_Exp(ref string T,ref string N)
        {
            //FIRST(<OR_Exp>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST  }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant"||
                tokenList[index].CP == "bool_constant"
            )
            {
                string T2 = "";
                string N2 = "";
                //<OR_Exp>  <AND_Exp> <OR_Exp2>
                if (AND_Exp(ref T2,ref N2))
                {
                    if (OR_Exp2(ref T,T2,ref N,N2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool OR_Exp2(ref string T3,string T,ref string N,string N2)
        {
            //FIRST(<OR_Exp2>) = {|| , Null}
            if (tokenList[index].CP == "||")
            {
                //<OR_Exp2>  || <AND_Exp> <OR_Exp2> | Null
                if (tokenList[index].CP == "||")
                {
                    string Op = tokenList[index].CP;
                    index++;
                    string RT = "";
                    string RTN = "";
                    if (AND_Exp(ref RT,ref RTN))
                    {
                        string T2 = semanticAnalyzer.comp(T, RT, Op);
                        if(T2 == "invalid")
                        {
                            addError("Type Mismatch at "+T+" "+Op+" "+T);
                        }
                        string TN = icg.CreateTemp();
                        icg.gen(TN + " = " + N2 + " " + Op + " " + RTN);
                        if (OR_Exp2(ref T3,T2,ref N,RTN))
                        {
                            return true;
                        }
                    }
                }
            }

            //FOLLOW(<OR_Exp2>) = { , ,  ; , )}
            if (tokenList[index].CP == "," ||
                tokenList[index].CP == ";" ||
                tokenList[index].CP == "]" ||
                tokenList[index].CP == "}" ||
                tokenList[index].CP == ")")
            {
                N = N2;
                T3 = T;
                return true;
            }
            return false;
        }

        private bool AND_Exp(ref string T2,ref string N)
        {
            //FIRST(<AND_Exp>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST  }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant"||
                tokenList[index].CP == "bool_constant"
            )
            {
                string T = "";
                string N1 = "";
                //<AND_Exp>  <ROP> <AND_Exp2>
                if (ROP(ref T,ref N1))
                {

                    if (AND_Exp2(ref T2,T,ref N,N1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool AND_Exp2(ref string T3,string T,ref string N,string N1)
        {
            //FIRST(<AND_Exp2>) = {&& , Null}
            if (tokenList[index].CP == "&&")
            {
                //<AND_Exp2>  && <ROP> <AND_Exp2> | Null
                if (tokenList[index].CP == "&&")
                {
                    string Op = tokenList[index].CP;
                    index++;
                    string T2 = "";
                    string RTN = "";
                    if (ROP(ref T2,ref RTN))
                    {
                        string T4 = semanticAnalyzer.comp(T, T2, Op);
                        if(T4 == "invalid")
                        {
                            addError("Type Mismatch at " + T + " " + Op + " " + T2);
                        }
                        string TN = icg.CreateTemp();
                        icg.gen(TN + " = " + N1 + " " + Op + " " + RTN);
                        if (AND_Exp2(ref T3,T4,ref N,RTN))
                        {
                            return true;
                        }
                    }
                }
            }
            ////FOLLOW(<AND_Exp2>) = {||, , ,  ; , )}

            if (tokenList[index].CP == "||" ||
                tokenList[index].CP == "," ||
                tokenList[index].CP == ";" ||
                tokenList[index].CP == "]" ||
                tokenList[index].CP == "}" ||
                tokenList[index].CP == ")")
            {
                N = N1;
                T3 = T;
                return true;
            }
            return false;
        }


        private bool ROP(ref string RT,ref string N)
        {
            //FIRST(<ROP>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST  }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant"||
                tokenList[index].CP == "bool_constant"
            )
            {
                string T = "";
                string N1 = "";
                //<ROP>  <E> <ROP2>
                if (E(ref T,ref N1))
                {
                    if (ROP2(ref RT,T,ref N,N1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ROP2(ref string RT,string T,ref string N,string N1)
        {
            //FIRST(<ROP2>) = {ROP , Null}
            if (tokenList[index].CP == "Rel_Op") //can be '=' TEMP
            {
                //<ROP2>  ROP <E> <ROP2> | Null
                if (tokenList[index].CP == "Rel_Op")
                {
                    string Op = tokenList[index].VP;
                    index++;
                    string T2 = "";
                    string RTN = "";
                    if (E(ref T2,ref RTN))
                    {
                        string T3 = semanticAnalyzer.comp(T, T2, Op);
                        if(T3 == "invalid")
                        {
                            addError("Type Mismatch at " + T + " " + Op + " " + T2);
                        }
                        string TN = icg.CreateTemp();
                        icg.gen(TN + " = " + N1 + " " + Op + " " + RTN);
                        if (ROP2(ref RT,T3,ref N,RTN))
                        {
                            return true;
                        }
                    }
                }
            }

            ////FOLLOW(<ROP2>) = {&& ,||, , ,  ; , )}
            if (tokenList[index].CP == "&&" ||
                tokenList[index].CP == "||" ||
                tokenList[index].CP == "," ||
                tokenList[index].CP == ";" ||
                tokenList[index].CP == "]" ||
                tokenList[index].CP == "}" ||
                tokenList[index].CP == ")")
            {
                N = N1;
                RT = T;
                return true;
            }
            return false;
        }

        private bool E(ref string T2,ref string N)
        {
            //FIRST(<E>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST  }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant"||
                tokenList[index].CP == "bool_constant"
            )
            {
                string T1 = "";
                string N1 = "";
                //<E>  <T> <E2>
                if (T(ref T1,ref N1))
                {
                    if (E2(ref T2,T1,ref N,N1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool E2(ref string T3,string T,ref string N,string N1)
        {
            //FIRST(<E2 >) = {Plus_Minus , Null}
            if (tokenList[index].CP == "P_M")
            {
                //<E2 >  Plus_Minus <T > <E2> | Null
                if (tokenList[index].CP == "P_M")
                {
                    string Op = tokenList[index].VP;
                    index++;
                    string T2 = "";
                    string RTN = "";
                    if (this.T(ref T2,ref RTN))
                    {
                        string T1 = semanticAnalyzer.comp(T, T2, Op);
                        if(T1 == "invalid")
                        {
                            addError("Type Mismatch at " + T + " " + Op + " " + T2);
                        }
                        string TN = icg.CreateTemp();
                        icg.gen(TN + " = " + N1 + " " + Op + " " + RTN);
                        if (E2(ref T3,T1,ref N,RTN))
                        {
                            return true;
                        }
                    }
                }
            }
            ////FOLLOW(<E2>) = {ROP , && ,||, , ,  ; , )}
            if (tokenList[index].CP == "Rel_Op" || // maybe '=' TEMP
                tokenList[index].CP == "&&" ||
                tokenList[index].CP == "||" ||
                tokenList[index].CP == "," ||
                tokenList[index].CP == ";" ||
                tokenList[index].CP == "]" ||
                tokenList[index].CP == "}" ||
                tokenList[index].CP == ")")
            {
                N = N1;
                T3 = T;
                return true;
            }
            return false;
        }

        private bool T(ref string T,ref string N)
        {
            //FIRST(<T>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST  }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant"||
                tokenList[index].CP == "bool_constant"
            )
            {
                string T1 = "";
                string N1 = "";
                //<T>  <F> <T2>
                if (F(ref T1,ref N1))
                {
                    if (T2(ref T,T1,ref N,N1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool T2(ref string T,string T1,ref string N,string N1)
        {
            //FIRST(<T2>) = { M_D_M , Null}
            if (tokenList[index].CP == "D_M" || tokenList[index].CP == "*")
            {
                //<T2>  M_D_M <F> <T2> | Nulll
                if (tokenList[index].CP == "D_M" || tokenList[index].CP == "*")
                {
                    string Op = tokenList[index].VP;
                    index++;
                    string T22 = "";
                    string RTN = "";
                    if (F(ref T22,ref RTN))
                    {
                        string T3 = semanticAnalyzer.comp(T1, T22, Op);
                        if(T3=="invalid")
                        {
                            addError("Type Mismatch at " + T1 + " " + Op + " " + T22);
                        }
                        string TN = icg.CreateTemp();
                        icg.gen(TN + " = " + N1 + " " + Op + " " + RTN);
                        if (T2(ref T,T3,ref N,RTN))
                        {
                            return true;
                        }
                    }
                }
            }
            ////FOLLOW(<T2>) = { Plus_Minus , ROP , && ,||, , ,  ; , )}

            if (tokenList[index].CP == "P_M" ||
                tokenList[index].CP == "Rel_Op" || // maybe '=' TEMP
                tokenList[index].CP == "&&" ||
                tokenList[index].CP == "||" ||
                tokenList[index].CP == "," ||
                tokenList[index].CP == ";" ||
                tokenList[index].CP == "]" ||
                tokenList[index].CP == "}" ||
                tokenList[index].CP == ")")
            {
                N = N1;
                T = T1;
                return true;
            }
            return false;
        }

        private bool F(ref string RT,ref string RN)
        {
            //FIRST(<F>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST  }
            //FIRST(<F>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST , ! , ( , inc_dec }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant"||
                tokenList[index].CP == "bool_constant" ||
                tokenList[index].CP == "INC_DEC" ||
                tokenList[index].CP == "!" ||
                tokenList[index].CP == "("
                )
            {
                string constT = "";
                //<F>  ID | <CONST>
                //<F> ID <id_op>  |<Const> |!<F> | (<Exp>) | Inc_Dec  ID<inc_dec_list>
                if (tokenList[index].CP == "ID")
                {
                    string N = tokenList[index].VP;
   //                 RN = N;
                    string T = getType(N, "Undeclared Variable");
                    index++;
                    if (id_op(ref RT, N, T,ref RN))
                    {
                        return true;
                    }
                }
                else if (CONST(ref constT))
                {
                    RN = tokenList[index - 1].VP;
                    RT = constT;
                    return true;
                }
                else if (tokenList[index].CP == "!")
                {
                    string OP = tokenList[index].VP;
                    string T = "";
                    string NN = "";
                    index++;
                    if (F(ref T,ref NN))
                    {
                        if (T != "aur_bool")
                        {
                            addError("Type Mismatch !" + T);
                        }
                        RT = T;
                        RN = T;
                        return true;
                    }
                }
                else if (tokenList[index].CP == "(")
                {

                    string T = "";
                    string Nexp = "";
                    index++;
                    if (Exp(ref T,ref RN))
                    {
                        if (tokenList[index].CP == ")")
                        {

                            index++;
                            RT = T;
                            RN = T;
                            return true;
                        }
                    }
                }
                else if (tokenList[index].CP == "INC_DEC")
                {
                    index++;
                    if (tokenList[index].CP == "ID")
                    {
                        string N = tokenList[index].VP;
                        string T = getType(N, "Undeclared");
                        string T2 = "";
                        string N1 = "";
                        index++;
                        if (inc_dec_list(ref T2, N, T,ref N1))
                        {
                            RN = N1;
                             return true;
                        }
                    }
                }
            }
             return false;
        }

        private bool inc_dec_list(ref string RT, string N, string T1,ref string RN)
        {
            
            //FIRST(<inc_dec_list>) = { [ , . , Null}
            if (tokenList[index].CP == "[" ||
                tokenList[index].CP == ".")
            {
                //<inc_dec_list>  [<Exp>] | .ID[<Exp>] |Null 
                if (tokenList[index].CP == "[")
                {

                    index++;
                    string ET = "";
                    string Nexp = "";
                    if (Exp(ref ET,ref Nexp))
                    {
                        if (tokenList[index].CP == "]")
                        {
                            index++;
                            if (getType(N, "Undeclared Array") != "invalid")
                            {
                                RT = T1;
                                RN = N;
                            }
                            return true;
                        }
                    }
                }
                else if (tokenList[index].CP == ".")
                {
                    if (getType(N, "Undeclared Object") != "invalid")
                    {
                        RT = T1;
                        RN = N;
                    }
                    index++;
                    if (tokenList[index].CP == "ID")
                    {
                        string N1 = tokenList[index].VP;
                        index++;
                        if (tokenList[index].CP == "[")
                        {
                            
                            index++;
                            string ET = "";
                            string Nexp = "";
                            if (Exp(ref ET,ref Nexp))
                            {
                                if (tokenList[index].CP == "]")
                                {
                                    
                                    index++;
                                    if (getType(N1, "Undeclared Array") != "invalid")
                                    {
                                        RT = T1;
                                        RN = N;
                                    }
                                     return true;
                                }
                            }
                        }
                    }
                }
            }
            //FOLLOW(<inc_dec_list>) = {M_D_M , Plus_Minus , ROP , && ,||, ,  , ) , } , ] , ;}
            else if ((tokenList[index].CP == "D_M" || tokenList[index].CP == "*") ||
                    tokenList[index].CP == "P_M" ||
                    tokenList[index].CP == "Rel_Op" ||
                    tokenList[index].CP == "&&" ||
                    tokenList[index].CP == "||" ||
                    tokenList[index].CP == "," ||
                    tokenList[index].CP == ")" ||
                    tokenList[index].CP == "}" ||
                    tokenList[index].CP == "]" ||
                    tokenList[index].CP == ";")
            {


                if (getType(N, "Undeclared variable " + N) != "invalid")
                {
                    RT = T1;
                    RN = N;
                }
                return true;
            }
             return false;
        }

        private bool id_op(ref string RT, string N, string T,ref string RN)
        {
            //FIRST(<id_op>) = { Null , ( , [ , . , inc_dec}
            if (tokenList[index].CP == "(" ||
                tokenList[index].CP == "[" ||
                tokenList[index].CP == "." ||
                tokenList[index].CP == "INC_DEC")
            {
                //<id_op>  Null | <Method_Call_1> | [ <Exp> ] |<Member_exp> |  Inc_Dec 

                if (Method_Call(ref RT, N))
                {
                    RN = N;
                    return true;
                }
                else if (tokenList[index].CP == "[")
                {

                    index++;
                    string ET = "";
                    string Nexp = "";
                    if (Exp(ref ET,ref Nexp))
                    {
                        if (tokenList[index].CP == "]")
                        {

                            if (getType(N, "Undeclared Array") != "invalid")
                            {
                            }
                            RT = T;
                            RN = N;
                            index++;
                            return true;
                        }
                    }
                }
                else if (Member_exp(ref RT, N,ref RN))
                {
                     return true;
                }
                else if (tokenList[index].CP == "INC_DEC")
                {
                    if (getType(N, "Undeclared variable") != "invalid")
                    {
                    }
                    index++;
                    RT = T;
                    RN = N;
                    return true;
                }
            }

            ////FOLLOW(<id_op>) = {M_D_M , Plus_Minus , ROP , && ,||, ,  , ) , } , ] , ;}
            else if ((tokenList[index].CP == "D_M" || tokenList[index].CP == "*" )||
                    tokenList[index].CP == "P_M" ||
                    tokenList[index].CP == "Rel_Op" ||
                    tokenList[index].CP == "&&" ||
                    tokenList[index].CP == "||" ||
                    tokenList[index].CP == "," ||
                    tokenList[index].CP == ")" ||
                    tokenList[index].CP == "}" ||
                    tokenList[index].CP == "]" ||
                    tokenList[index].CP == ";")
            {
                if(getType(N,"Undeclared Variable") != "invalid")
                { }
                RN = N;
                RT = T;
                return true;
            }
            return false;
        }

        private bool Member_exp(ref string RT, string N,ref string RN)
        {
            
            //FIRST(<Member_exp>) = { . }
            if (tokenList[index].CP == ".")
            {
                //<Member_exp> -> .ID < Member_exp_2>
                if (tokenList[index].CP == ".")
                {



                    getType(N, "Undeclared Object");
                    index++;
                    if (tokenList[index].CP == "ID")
                    {
                        string N1 = tokenList[index].VP;
                        string T1 = getType(N1, "Undeclared variable");

                        index++;
                        if (Member_exp_2(ref RT, N1, T1,ref RN))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool Member_exp_2(ref string RT, string N, string T,ref string RN)
        {
            
            //FIRST(< Member_exp_2>) = {Null , ( , [}
            if (tokenList[index].CP == "(" ||
                tokenList[index].CP == "[")
            {
                //< Member_exp_2> -> Null | <Method_Call_1> | [<Exp>]
                if (Method_Call(ref RT, N))
                {
                    RN = N;
                   return true;
                }
                else if (tokenList[index].CP == "[")
                {
                    //currentNode = currentNode.Parent;
                    index++;
                    string ET = "";
                    string Nexp = "";
                    if (Exp(ref ET,ref Nexp))
                    {
                        if (tokenList[index].CP == "]")
                        {
                            //currentNode = currentNode.Parent;
                            RT = T;
                            RN = N;
                            getType(N, "Undeclared Array");
                            index++;
                            return true;
                        }
                    }
                }
            }

            //FOLLOW(<Member_exp2>) = {M_D_M , Plus_Minus , ROP , && ,||, ,  , ) , } , ] , ;}
            else if (tokenList[index].CP == "D_M" || tokenList[index].CP == "*" ||
                    tokenList[index].CP == "P_M" ||
                    tokenList[index].CP == "Rel_Op" ||
                    tokenList[index].CP == "&&" ||
                    tokenList[index].CP == "||" ||
                    tokenList[index].CP == "," ||
                    tokenList[index].CP == ")" ||
                    tokenList[index].CP == "}" ||
                    tokenList[index].CP == "]" ||
                    tokenList[index].CP == ";")
            {


                RN = N;
                RT = T;
                getType(N, "Undeclared Variable");

                return true;
            }
            return false;
        }

        private bool CONST(ref string T)
        {
            if (tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant" ||
                tokenList[index].CP == "bool_constant")
            {
                if (tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant" ||
                tokenList[index].CP == "bool_constant")
                {
                    if (tokenList[index].CP == "num_constant")
                    {
                        T = "number_constant";
                    }
                    else if (tokenList[index].CP == "point_constant")
                    {
                        T = "point_constant";
                    }
                    else if (tokenList[index].CP == "sent_constant")
                    {
                        T = "sent_constant";
                    }
                    else if (tokenList[index].CP == "single_constant")
                    {
                        T = "single_constant";
                    }
                    else if (tokenList[index].CP == "bool_constant")
                    {
                        T = "bool_constant";
                    }
                    index++;
                    return true;
                }
            }
            return false;
        }

        private bool Param_List(ref string AL, string PL, ref string AQ)
        {

            string T = "";
            int PQ = 0;
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant" ||
                tokenList[index].CP == "bool_constant")
                    
            {
                    
                    string Nexp = "";
                if (Exp(ref T,ref Nexp))
                {
                    PL += T;
                    icg.gen("PARAM " + Nexp);
                    PQ = 1;
                    if (Param_List1(ref AL, PL, ref AQ, PQ))
                    {
                        return true;
                    }
                }
            }

            if (tokenList[index].CP == ")")
            {
                AQ = PQ.ToString();
                AL = PL;
                return true;
            }
            return false;
        }

        private bool Param_List1(ref string AL, string PL, ref string AQ, int PQ)
        {
            string T = "";
            if (tokenList[index].CP == ",")
            {
                index++;
                string Nexp = "";
                if (Exp(ref T,ref Nexp))
                {
                    PL += "," + T;
                    icg.gen("PARAM "+Nexp);
                    PQ++;
                    if (Param_List1(ref AL, PL, ref AQ, PQ))
                    {
                        return true;
                    }
                }
            }
            if (tokenList[index].CP == ")")
            {
                AQ = PQ.ToString();
                AL = PL;
                return true;
            }
            return false;
        }

        private bool List_Param_Dec(ref string AL, string PL, ref string NAL, string NPL)
        {
            if (tokenList[index].CP == "DT")
            {
                if (tokenList[index].CP == "DT")
                {
                    String T = tokenList[index].VP;
                    PL += T;
                    index++;
                    if (tokenList[index].CP == "ID")
                    {

                        String N = tokenList[index].VP;
                        NPL += N;
                        index++;
                        if (List_Param_Dec1(ref AL, PL, ref NAL, NPL))
                        {
                            return true;
                        }
                    }
                }
            }
            if (tokenList[index].CP == ")")
            {

                NAL = NPL;
                AL = PL;
                return true;
            }
            return false;
        }

        private bool List_Param_Dec1(ref string AL, string PL, ref string NAL, string NPL)
        {
            if (tokenList[index].CP == ",")
            {
                if (tokenList[index].CP == ",")
                {
                    index++;
                    if (tokenList[index].CP == "DT")
                    {
                        String T = tokenList[index].VP;
                        PL += "," + T;
                        index++;
                        if (tokenList[index].CP == "ID")
                        {
                            String N = tokenList[index].VP;
                            NPL += "," + N;
                            index++;
                            if (List_Param_Dec1(ref AL, PL, ref NAL, NPL))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            if (tokenList[index].CP == ")")
            {
                AL = PL;
                return true;
            }
            return false;
        }
        

        private bool M_ST(ref string T)
        {
            //FIRST(<M_ST>) = { jabtak , DT , Barbar , agar , return ,  inc_dec , ID , break , continue, this , Null}
            if (tokenList[index].CP == "while" ||
                tokenList[index].CP == "DT" ||
                tokenList[index].CP == "barbar" ||
                tokenList[index].CP == "agar" ||
                tokenList[index].CP == "return" ||
                tokenList[index].CP == "INC_DEC" ||
                tokenList[index].CP == "ID" ||
                tokenList[index].CP == "break" ||
                tokenList[index].CP == "continue" ||
                tokenList[index].CP == "this")
            {
                //<M_ST>   <S_ST><M_ST> | Null
                if (S_ST(ref T))
                {
                    if (M_ST(ref T))
                    {
                        return true;
                    }
                }
            }
            ////FOLLOW(<M_ST>) = { } }
            if (tokenList[index].CP == "}")
            {
                return true;
            }
            return false;
        }

        private bool S_ST(ref string T)
        {
            //FIRST(S_ST) = {while , DT , Barbar , agar , return ,  inc_dec , ID , break , continue , this}
            if (tokenList[index].CP == "while" ||
                tokenList[index].CP == "DT" ||
                tokenList[index].CP == "barbar" ||
                tokenList[index].CP == "agar" ||
                tokenList[index].CP == "return" ||
                tokenList[index].CP == "INC_DEC" ||
                tokenList[index].CP == "ID" ||
                tokenList[index].CP == "break" ||
                tokenList[index].CP == "continue" ||
                tokenList[index].CP == "this")
            {

                if (While())
                {
                    return true;
                }
                else if (tokenList[index].CP == "DT")
                {
                    string T1 = tokenList[index].VP;
                    index++;
                    if (S_St_DT(T1))
                    {
                        return true;
                    }
                }
                else if (Bar_Bar())
                {
                    return true;
                }
                else if (agar())
                {
                    return true;
                }
                else if (Return(ref T))
                {
                    return true;
                }
                else if (tokenList[index].CP == "INC_DEC")
                {
                    string Op = tokenList[index].VP;
                    index++;
                    if (tokenList[index].CP == "ID")
                    {
                        string N = tokenList[index].VP;
                        string T1 = getType(N, "Undeclared Variable " + N);
                        index++;
                        string RN = "";
                        if (inc_dec_list(ref T, N, T1,ref RN))
                        {
                            if (tokenList[index].CP == ";")
                            {
                                index++;
                                return true;
                            }
                        }
                    }
                }
                else if (tokenList[index].CP == "ID")
                {
                    string N = tokenList[index].VP;
                    string T1 = getType(N, "Undeclared Variable " + N);
                    index++;
                    if (S_St_ID(N,T1))
                    {
                        return true;
                    }
                }
                else if (BREAK())
                {
                    return true;
                }
                else if (CONTINUE())
                {
                    return true;
                }
            }
            return false;
            
        }

        private bool S_St_ID(string N,string T)
        {
            //FIRST(<S_St_ID>) = {inc_dec , = , ID ,  .  , (  }
            if (tokenList[index].CP == "INC_DEC" ||
                tokenList[index].CP == "AOP" ||
                tokenList[index].CP == "ID" ||
                tokenList[index].CP == "." ||
                tokenList[index].CP == "(")
            {
                //<S_St_ID>  inc_dec | <Assign_Op> | <Object_link> | <Object_Call> | <Method_Call_1>
                if (tokenList[index].CP == "INC_DEC")
                {
                    if(!semanticAnalyzer.MethodVariableLookUp(N,semanticAnalyzer.getCurrentScope()))
                    {
                        addError("Undeclared Variable");
                    }
                    string Op = tokenList[index].VP;
                    index++;
                    if(!semanticAnalyzer.Comp(T,Op))
                    {
                        addError("Compatiblity Error at " + T + " " + Op);
                    }
                    return true;
                }
                else if (Assign_Op(T,N))
                {
                    if (!semanticAnalyzer.MethodVariableLookUp(N, semanticAnalyzer.getCurrentScope()))
                    {
                        addError("Undeclared Variable");
                    }
                    return true;
                }
                //else if (Object_link())
                //{
                //    return true;
                //}
                else if (Object_Call(T))
                {
                    T = getType(N, "Undeclared Object");

                    if (tokenList[index].CP == ";")
                    {
                        index++;
                        return true;
                    }
                    return true;
                }
                else if (Method_Call(ref T,N))
                {
                    return true;
                }
                else if (tokenList[index].CP == "[")
                {
                    T = getType(N, "Undeclared Array "+N);
                    index++;
                    string T1 = "";
                    string Nexp = "";
                    if (Exp(ref T1,ref Nexp))
                    {
                        if (tokenList[index].CP == "]")
                        {
                            index++;
                            if (Assign_Op(T1,N))
                            {
                                 return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool S_St_DT(string T)
        {
            //FIRST(<S_St_DT>) = {ID , void , DT , [}
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "void" ||
                tokenList[index].CP == "DT" ||
                tokenList[index].CP == "[")
            {
                //<S_St_DT>  ID <S_St_DT2> | <Method_DEC> | <Array_DEC>
                if (tokenList[index].CP == "ID")
                {
                    string N = tokenList[index].VP;
                    semanticAnalyzer.insertVariables(N, T, semanticAnalyzer.getCurrentScope());
                    index++;
                    if (S_St_DT2(T,N))
                    {
                        return true;
                    }
                }
                else if (Array_DEC("","",T))
                {
                    return true;
                }
            }
            return false;
        }

        private bool S_St_DT2(string T,string N)
        {
            //FIRST(<S_St_DT2>) = { = }
            if (tokenList[index].CP == "AOP" || tokenList[index].CP == "," || tokenList[index].CP == ";")
            {
                //<S_St_DT2>  <Variable_Link2> 
                if (Variable_Link2(T,N))
                {
                    return true;
                }
            }
            return false;
        }


        //Method Declaration
        private bool Method_Link(string AM, string TM, string RT, string N)
        {
            //FIRST(<Method_Link 3>)
            if (tokenList[index].CP == "(")
            {
                string PL = "", NPL = "";
                string AL = "", NAL = "";
                if (tokenList[index].CP == "(")
                {
                    semanticAnalyzer.createScope();
                    index++;
                    if (List_Param_Dec(ref AL, PL, ref NAL, NPL))
                    {
                        if (tokenList[index].CP == ")")
                        {
                            string T = AL + '>' + RT;
                            MethodT newM = new MethodT(N, T, AM, TM, true);
                            semanticAnalyzer.insertMember(newM);
                            if(AL != "")
                            {
                                string[] arr = AL.Split(',');
                                string[] Narr = NAL.Split(',');
                                for(int i =0;i<arr.Length;i++)
                                {
                                    semanticAnalyzer.insertVariables(Narr[i], arr[i], semanticAnalyzer.getCurrentScope());
                                }

                            }
                            index++;
                            inMethod = true;
                            if (tokenList[index].CP == "{")
                            {
                                icg.gen(semanticAnalyzer.getCurrentClass() + "_"+ N +"_"+AL+" Proc");
                                index++;
                                string T1 = "";
                                if (M_ST(ref T1))
                                {
                                    if (tokenList[index].CP == "}")
                                    {
                                        index++;
                                        icg.gen(semanticAnalyzer.getCurrentClass() + "_" + N + "_" + AL + " endP");
                                        inMethod = false;
                                        semanticAnalyzer.deleteScope();
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool Object_link(string AM,string CN)
        {
            //FIRST(<Object_Link>) = {ID , [}
            if (tokenList[index].CP == "ID" || tokenList[index].CP == "[")
            {
                //<Object_Link> ID <Object_Creation_Exp>
                if (tokenList[index].CP == "ID")
                {
                    string Name = tokenList[index].VP;
                    if(semanticAnalyzer.MethodVariableLookUp(Name,semanticAnalyzer.getCurrentScope()))
                    {
                        addError("Object Redeclaration Error");
                    }
                    index++;
                    if (Object_Creation_Exp(CN,Name,AM))
                    {
                        return true;
                    }
                }
                else if (tokenList[index].CP == "[")
                {
                    index++;
                    if (tokenList[index].CP == "]")
                    {
                        index++;
                        if (tokenList[index].CP == "ID")
                        {
                            string N = tokenList[index].VP;
                            index++;
                            if (object_array_dec(CN, N, AM))
                            {
                                 return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool object_array_dec(string CN, string N1, string AM)
        {
            //FIRST(<object_array_dec>) = { = }
            if (tokenList[index].VP == "=")
            {
                //<object_array_dec>  = new ID[<Exp>]<obj_arr_dec1>
                if (tokenList[index].VP == "=")
                {
                    index++;
                    if (tokenList[index].CP == "new")
                    {
                        index++;
                        if (tokenList[index].CP == "ID")
                        {
                            string N2 = tokenList[index].VP;
                            if (!semanticAnalyzer.ClassLookUp(N2))
                            {
                                addError("Undeclared Class");
                            }
                            index++;
                            if (tokenList[index].CP == "[")
                            {
                               index++;
                                string ET = "";
                                string Nexp = "";
                                if (Exp(ref ET,ref Nexp))
                                {
                                    if (tokenList[index].CP == "]")
                                    {
                                        index++;
                                        if (inMethod)
                                        {
                                            semanticAnalyzer.insertVariables(CN, N1, semanticAnalyzer.getCurrentScope());
                                        }
                                        else
                                        {
                                            MethodT mem = new MethodT(CN, N1, AM, "", false);
                                            semanticAnalyzer.insertMember(mem);
                                        }
                                        if (obj_arr_dec1())
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

         private bool obj_arr_dec1()
        {
            //FIRST(<obj_arr_dec1>) = { ; , { }
            if(tokenList[index].CP == "{" ||
                tokenList[index].CP == ";")
            {
                //<obj_arr_dec1>  ;| {<obj_arr_dec2>
                if (tokenList[index].CP == ";")
                {
                    index++;
                    return true;
                }
                else if (obj_arr_dec2())
                {
                    return true;
                }
            }
            return false;
        }

         private bool obj_arr_dec2()
         {

             //FIRST(<obj_arr_dec2>) = { new , Null}
             if (tokenList[index].CP == "new")
             {
                 //<obj_arr_dec2>  new ID  (<Param>)<obj_arr_dec3>
                 if (tokenList[index].CP == "new")
                 {
                     index++;
                     if (tokenList[index].CP == "ID")
                     {
                         string N = tokenList[index].VP;
                         if (!semanticAnalyzer.ClassLookUp(N))
                         {
                             addError("Undeclared Class");
                         }
                         index++;
                         if (tokenList[index].CP == "(")
                         {

                             string PL = "", AL = "";
                             string NAL = "";
                             index++;
                             if (Param_List(ref AL, PL, ref NAL))
                             {
                                 if (tokenList[index].CP == ")")
                                 {

                                     index++;

                                     if (obj_arr_dec3())
                                     {
                                         return true;
                                     }
                                 }
                             }
                         }
                     }
                 }
             }
             return false;
         }

         private bool obj_arr_dec3()
         {
             if (tokenList[index].CP == "," ||
                 tokenList[index].CP == "}")
             {
                 if (tokenList[index].CP == ",")
                 {
                     index++;
                     if (obj_arr_dec2())
                     {
                         return true;
                     }
                 }
                 else if (tokenList[index].CP == "}")
                 {
                     index++;
                     if (tokenList[index].CP == ";")
                     {
                         index++;
                         return true;
                     }
                 }
             }
             return false;
         }

        private bool BREAK()
        {
            //FIRST(<Break>) = {break}
            if (tokenList[index].CP == "break")
            {
                //<Break>  break ;
                if (tokenList[index].CP == "break")
                {
                    index++;
                    if (tokenList[index].CP == ";")
                    {
                        index++;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CONTINUE()
        {
            //FIRST(<Break>) = {break}
            if (tokenList[index].CP == "continue")
            {
                //<Break>  break ;
                if (tokenList[index].CP == "continue")
                {
                    index++;
                    if (tokenList[index].CP == ";")
                    {
                        index++;
                        return true;
                    }
                }
            }
            return false;
        }

 //this

        private bool Return(ref string T)
        {
            //FIRST(<Return>) = {return}
            if (tokenList[index].CP == "return")
            {
                //<Return>  return <Exp> ;
                if (tokenList[index].CP == "return")
                {
                    index++;
                    string Nexp = "";
                    if (Exp(ref T,ref Nexp))
                    {
                        if (tokenList[index].CP == ";")
                        {
                            index++;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool agar()
        {
            //FIRST(<agar>) = {agar}
            if (tokenList[index].CP == "agar")
            {
                //<agar>  agar (<Exp>) {<M_ST>} <yaphir>
                if (tokenList[index].CP == "agar")
                {
                    index++;
                    string T = "";
                    string L1 = icg.CreateLabel();
                    if (tokenList[index].CP == "(")
                    {
                        index++;
                        string Nexp = "";
                        if (Exp(ref T,ref Nexp))
                        {
                            if (T != "bool_constant" || T != "bool")
                            {
                                addError("Type Error");
                            }
                            if (tokenList[index].CP == ")")
                            {
                                index++;
                                string tempvar = icg.CreateTemp();
                                icg.gen(tempvar +" = "+ Nexp);
                                icg.gen("if("+tempvar+" == false) jmp "+ L1);

                                if (tokenList[index].CP == "{")
                                {
                                    semanticAnalyzer.createScope();
                                    index++;
                                    string T1 = "";
                                    if (M_ST(ref T1))
                                    {
                                        if (tokenList[index].CP == "}")
                                        {
                                            string L2 = icg.CreateLabel();
                                            icg.gen("jmp " + L2);
                                            semanticAnalyzer.deleteScope();
                                            index++;
                                            if (yaphir(L1,L2))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool yaphir(string L1,string L2)

            //<yaphir>  yaphir (<Exp>) {<M_ST>}<yaphir> | <O_Else>
        {
            //FIRST(<yaphir>) = {yaphir}
            
            if (tokenList[index].CP == "yaphir")
            {
                //icg.gen("jmp " + L2);
                string L3 = icg.CreateLabel();
                if (tokenList[index].CP == "yaphir")
                {
                    index++;
                    icg.gen(L1 + ":");
                    if (tokenList[index].CP == "(")
                    {
                        index++;
                        string T = "";
                        string Nexp = "";
                        if (Exp(ref T,ref Nexp))
                        {
                            if (T != "bool_constant" || T != "bool")
                            {
                                addError("Type Error");
                            }
                            if (tokenList[index].CP == ")")
                            {
                                index++;
                                string TN1 = icg.CreateTemp();
                                icg.gen(TN1 + " = " + Nexp);
                                
                                icg.gen("if(" + TN1 + " == false) jmp " + L3);
                                if (tokenList[index].CP == "{")
                                {
                                    semanticAnalyzer.createScope();
                                    index++;
                                    string T1 = "";
                                    if (M_ST(ref T1))
                                    {
                                        if (tokenList[index].CP == "}")
                                        {
                                            semanticAnalyzer.deleteScope();
                                            index++;
                                            icg.gen("jmp " + L2);
                                            if (yaphir(L3,L2))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (O_Else(L1,L2))
            {
                return true;
            }
            return false;
        }
        

        private bool O_Else(string L1,string L2)
        {
            //FIRST(<O_Else>) = {warna , Null}
            if (tokenList[index].CP == "warna")
            {
                //<O_Else>  warna {<M_ST>} | Null
                if (tokenList[index].CP == "warna")
                {
                    index++;
                    icg.gen(L1 + ":");
                    if (tokenList[index].CP == "{")
                    {
                        semanticAnalyzer.createScope();
                        index++;
                        string T1 = "";
                        if (M_ST(ref T1))
                        {
                            if (tokenList[index].CP == "}")
                            {
                                semanticAnalyzer.deleteScope();
                                index++;
                                icg.gen(L2 + ":");
                                return true;
                            }
                        }
                    }
                }
            }
            //FOLLOW(<O_Else>) = { while , DT , Barbar , agar , return ,  inc_dec , ID , break , continue, this , }}
            if (tokenList[index].CP == "while" ||
                tokenList[index].CP == "DT" ||
                tokenList[index].CP == "barbar" ||
                tokenList[index].CP == "agar" ||
                tokenList[index].CP == "return" ||
                tokenList[index].CP == "INC_DEC" ||
                tokenList[index].CP == "ID" ||
                tokenList[index].CP == "break" ||
                tokenList[index].CP == "continue" ||
                tokenList[index].CP == "}")
            {
                icg.gen(L1 + ":");
                return true;
            }
            return false;
        }

        
        private bool Array_DEC(string AM,string TM,string RT)
        {
            //FIRST(<Array_DEC>) = {[}
            if (tokenList[index].CP == "[")
            {
                //<Array_DEC>   [] ID <INIT_Array>
                if (tokenList[index].CP == "[")
                {
                    index++;
                    if (tokenList[index].CP == "]")
                    {
                        index++;
                        RT += "[]";
                        if (tokenList[index].CP == "ID")
                        {
                            string N = tokenList[index].VP;
                            if (inMethod)
                            {
                                semanticAnalyzer.insertVariables(N, RT, semanticAnalyzer.getCurrentScope());
                            }
                            else
                            {
                                MethodT mem = new MethodT(N,RT,AM,TM,false);
                                semanticAnalyzer.insertMember(mem);
                            }
                            index++;
                            if (INIT_Array(RT))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool INIT_Array(string T)
        {
            //FIRST(<INIT_Array>) = {; , =}
            if (tokenList[index].CP == ";" ||
                tokenList[index].CP == "AOP")
            {
                //<INIT_Array>  ; | = new DT [<ID_Const>]<Array_const>
                if (tokenList[index].CP == ";")
                {
                    index++;
                    return true;
                }
                else if (tokenList[index].CP == "AOP")
                {
                    index++;
                    if (tokenList[index].CP == "new")
                    {
                        index++;
                        if (tokenList[index].CP == "DT")
                        {
                            string T1 = tokenList[index].VP;
                            index++;
                            if (tokenList[index].CP == "[")
                            {
                                string ET = "";
                                index++;
                                string Nexp = "";
                                if (Exp(ref ET,ref Nexp))
                                {
                                    if (tokenList[index].CP == "]")
                                    {
                                        T1 += "[]";
                                        if (T != T1)
                                        {
                                            addError("Array Type Mismatch " + T + " " + T1);
                                        }
                                        index++;
                                        if (Array_const())
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool Array_const()
        {
            //FIRST(<Array_const>) = {{ , ;}
            if (tokenList[index].CP == "{" ||
                tokenList[index].CP == ";")
            {
                //<Array_const>  <Array_C> | ;
                if (tokenList[index].CP == ";")
                {
                    index++;
                    return true;
                }
                else if (Array_C())
                {
                    return true;
                }
            }
            return false;
        }

        private bool Array_C()
        {
            // FIRST(<Array_C>) = { { }
            if (tokenList[index].CP == "{")
            {
                //<Array_C>  { <Const> <Array_C2>
                if (tokenList[index].CP == "{")
                {
                    index++;
                    string ET = "";
                    string Nexp = "";
                    if (Exp(ref ET,ref Nexp))
                    {
                        if (Array_C2())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool Array_C2()
        {
            //FIRST(<Array_C2>) = {, , } }
            if (tokenList[index].CP == "{" ||
                tokenList[index].CP == ",")
            {
                //<Array_C2>  , <Const> | } ;
                if (tokenList[index].CP == "}")
                {
                    index++;
                    if (tokenList[index].CP == ";")
                    {
                        index++;
                        return true;
                    }
                }
                else if (tokenList[index].CP == ",")
                {
                    index++;
                    string ET = "";
                    string Nexp = "";
                    if (Exp(ref ET,ref Nexp))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ID_Const(ref string T)
        {
            //FIRST(<ID_Const>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant" ||
                tokenList[index].CP == "bool_constant")
            {
                T = tokenList[index].VP;
                index++;
                return true;
            }
            return false;
        }

        //private bool Method_DEC()
        //{
        //    //FIRST(<Method_DEC>) = {DT , void}
        //    if (tokenList[index].CP == "DT" ||
        //        tokenList[index].CP == "void")
        //    {
        //        string T = "";
        //        //<Method_DEC>   <Return_Type>  ID <Method_Link 3>
        //        if (Return_Type(ref T))
        //        {
        //            if (tokenList[index].CP == "ID")
        //            {
        //                string N = tokenList[index].VP;
        //                index++;
        //                if (Method_Link3())
        //                {
        //                    return true;
        //                }
        //            }
        //        }

        //    }

        //    return false;
        //}

        private bool Return_Type(ref string T)
        {
            //FIRST(<Return_Type>) = { void, DT }
            if (tokenList[index].CP == "void" ||
                tokenList[index].CP == "DT")
            {
                T = tokenList[index].VP;
                index++;
                return true;
            }
            return false;
        }

        

        private bool While()
        {
            //FIRST(<while>) = {while}
            if (tokenList[index].CP == "while")
            {
                //<while>  while (<Exp>) <Body>
                if (tokenList[index].CP == "while")
                {
                    index++;
                    string L1 = icg.CreateLabel();
                    icg.gen(L1 + ":");
                    if (tokenList[index].CP == "(")
                    {
                        index++;
                        string T = "";
                        string Nexp = "";
                        if (Exp(ref T,ref Nexp))
                        {
                            if (tokenList[index].CP == ")")
                            {
                                if (T == "bool" || T == "bool_constant")
                                {
                                }
                                else
                                {
                                    addError("Error Type");
                                }
                                index++;
                                string L2 = icg.CreateLabel();
                                string TN1 = icg.CreateTemp();
                                icg.gen(TN1 + " = " + Nexp);
                                icg.gen("if(" + TN1 + " == false) jmp " + L2);
                                if (Body())
                                {
                                    icg.gen("jmp "+L1);
                                    icg.gen(L2 + ":");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool Bar_Bar()
        {
            if (tokenList[index].CP == "barbar")
            {
                //<Bar_Bar>  barbar(<F1>; <F2>; <F3>) <Body>
                if (tokenList[index].CP == "barbar")
                {
                    index++;
                    if (tokenList[index].CP == "(")
                    {
                        index++;
                        if (F1())
                        {
                            string LN = icg.CreateLabel();
                            icg.gen(LN + ":");
                            if (F2())
                            {
                                if (tokenList[index].CP == ";")
                                {
                                    index++;
                                    string LN1 = "";
                                    if (F3(ref LN1))
                                    {
                                        if (tokenList[index].CP == ")")
                                        {
                                            index++;
                                            if (Body())
                                            {
                                                icg.gen("jmp " + LN);
                                                icg.gen(LN1 + ":");
                                                return true;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool F1()
        {
            //FIRST(<F1>) = {DT , ID , Null}
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "DT")
            {
                //<F1>  <DEC> |ID <Assign_Op> | Null
                if (DEC())
                {
                    return true;
                }
                else if (tokenList[index].CP == "ID")
                {
                    string N = tokenList[index].VP;
                    string T = getType(N, "Undeclared Variable");
                    index++;
                    if (Assign_Op(T,N))
                    {
                        return true;
                    }
                }
            }
            //FOLLOW(<F1>) = { ; }
            else if (tokenList[index].CP == ";")
            {
                index++;
                return true;
            }
            return false;
        }

        private bool F3(ref string LN)
        {
            //FIRST(<F2>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST , Null }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant" ||
                tokenList[index].CP == "bool_constant")
            {
                string T = "";
                string Nexp = "";
                //<F2>  <Exp> <X> | Null
                if (Exp(ref T,ref Nexp))
                {
                    if(T != "bool_constant" || T!= "bool")
                    {
                        addError("Type Error");
                    }
                    LN = icg.CreateLabel();
                    icg.gen("if(" + Nexp + " == false) jmp " + LN);

                    if (X(ref LN))
                    {
                        return true;
                    }
                }
            }
            ////FOLLOW( ) ) = { ) }
            else if (tokenList[index].CP == ")")
            {
                return true;
            }
            
            return false;
        }

        private bool F2()
        {
            //FIRST(<F3>) = {inc_dec , ID , Null}
            if (tokenList[index].CP == "INC_DEC" ||
                tokenList[index].CP == "ID")
            {
                string LN = icg.CreateLabel();
                icg.gen(LN + ":");
                //<F3>  inc_dec ID | ID <F4>| Null
                if (tokenList[index].CP == "INC_DEC")
                {
                    index++;
                    if (tokenList[index].CP == "ID")
                    {
                        string N = tokenList[index].VP;
                        if(!semanticAnalyzer.MethodVariableLookUp(N, semanticAnalyzer.getCurrentScope()))
                        {
                            addError("Undeclared Variable ");
                        }
                        string TN = icg.CreateTemp();
                        icg.gen(TN + " = " + N + "+ 1");
                        icg.gen(N + " = " + TN);
                        index++;
                        return true;
                    }
                }
                else if (tokenList[index].CP == "ID")
                {
                    string N = tokenList[index].VP;
                    string T = getType(N, "Undeclared Variable");
                    index++;
                    if (F4(ref T,N))
                    {
                        return true;
                    }
                }
            }
            ////FOLLOW(<F3>) = { ) }
            else if (tokenList[index].CP == ";")
            {
                return true;
            }
            
            return false;
        }

        private bool F4(ref string T,string N)
        {
            //FIRST(<F4>) = {inc_dec , AOP}
            if (tokenList[index].CP == "INC_DEC" ||
                tokenList[index].CP == "AOP")
            {
                //<F4>  inc_dec | AOP <Exp>
                if (tokenList[index].CP == "INC_DEC")
                {
                    index++;
                    string TN = icg.CreateTemp();
                    icg.gen(TN + " = " + N + "+ 1");
                    icg.gen(N + " = " + TN);
                    return true;
                }
                else if (tokenList[index].CP == "AOP")
                {
                    string Op = tokenList[index].VP;
                    string RT = "";
                    index++;
                    string Nexp = "";
                    if (Exp(ref RT,ref Nexp))
                    {
                        string T1 = semanticAnalyzer.comp(T, RT, Op);
                        if(T1 == "invalid")
                        {
                            addError("Compatiblity Error at " + T + " " + Op + " " + RT);
                        }
                        string TN = icg.CreateTemp();
                        icg.gen(TN + " = " + Nexp);
                        icg.gen(N + " = " + TN);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool X(ref string LN)
        {
            //FIRST(<X>) = { , , Null}
            if (tokenList[index].CP == ",")
            {
                index++;
                string T = "";
                string Nexp = "";
                //<X>  , <Exp> <X> | Null
                if (Exp(ref T,ref Nexp))
                {
                    if (T != "bool_constant" || T != "bool")
                    {
                        addError("Type Error");
                    }
                    LN = icg.CreateLabel();
                    icg.gen("if(" + Nexp + " == false) jmp " + LN);

                    if (X(ref LN))
                    {
                        return true;
                    }
                }
            }

            ////FOLLOW(<X>) = { ) }

            if (tokenList[index].CP == ")")
            {
                return true;
            }
            return false;
        }

        private bool Body()
        {
            //FIRST(<Body>) = {; , { , jabtak , DT , Barbar , agar , return ,  inc_dec , ID , break , continue , this }
            if (tokenList[index].CP == "while" ||
                tokenList[index].CP == "DT" ||
                tokenList[index].CP == "barbar" ||
                tokenList[index].CP == "agar" ||
                tokenList[index].CP == "return" ||
                tokenList[index].CP == "INC_DEC" ||
                tokenList[index].CP == "ID" ||
                tokenList[index].CP == "break" ||
                tokenList[index].CP == "continue" ||
                tokenList[index].CP == "this" ||
                tokenList[index].CP == "," ||
                tokenList[index].CP == ";" ||
                tokenList[index].CP == "{")
            {
                string T = "";
                //<Body>  ; | <S_ST> | {<M_ST>}
                if (tokenList[index].CP == ";")
                {
                    index++;
                }

                else if (S_ST(ref T))
                {
                    return true;
                }
                else if (tokenList[index].CP == "{")
                {
                    semanticAnalyzer.createScope();
                    index++;
                    if (M_ST(ref T))
                    {
                        if (tokenList[index].CP == "}")
                        {
                            semanticAnalyzer.deleteScope();
                            index++;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool DEC()
        {
            //FIRST(<DEC>) = { DT}
            if (tokenList[index].CP == "DT")
            {
                //<DEC>  DT <Variable_Link>
                if (tokenList[index].CP == "DT")
                {
                    string T = tokenList[index].VP;
                    index++;
                    if (Variable_Link(T))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Variable_Link(string T)
        {
            //FIRST(<Variable_Link>) = {ID} 
            if (tokenList[index].CP == "ID")
            {
                //<Variable_Link>  ID <Varaiable_Link2>
                if (tokenList[index].CP == "ID")
                {
                    string N = tokenList[index].VP;
                    int S = semanticAnalyzer.getCurrentScope();
                    semanticAnalyzer.insertVariables(N, T, S);
                    index++;
                    if (Variable_Link2(T,N))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Variable_Link2(string T,string N)
        {
            //FIRST(<Variable_Link2>  ) = {=, , , ;}
            if (tokenList[index].CP == "AOP" ||
                tokenList[index].CP == "," ||
                tokenList[index].CP == ";")
            {
                //<Variable_Link2>   =  <Variable_Value>| <LIST>
                if (tokenList[index].CP == "AOP")
                {
                    string Op = tokenList[index].VP;
                    index++;
                    if (Variable_Value(T, Op,N))
                    {
                        return true;
                    }
                }
                else if (LIST(T))
                {
                    return true;
                }
            }
            return false;
        }

        private bool Variable_Value(string T, string Op,string N)
        {
            //FIRST(<Variable_Value>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant" ||
                tokenList[index].CP == "bool_constant" ||
                tokenList[index].CP == "{"
                )
            {
                string T2 = "";
                string Nexp = "";
                //<Variable_Value>   <Exp><LIST>  	
                if (Exp(ref T2,ref Nexp))
                {
                    string TN = icg.CreateTemp();
                    icg.gen(TN + " = " + Nexp);
                    icg.gen(N + " = " + TN);
                    if (semanticAnalyzer.comp(T, T2, Op) == "invalid")
                    {
                        addError("Type Mismatch at " + T + " " + Op + " " + T2);
                    }
                    if (LIST(T))
                    {
                        return true;
                    }
                }
                else if (tokenList[index].CP == "{")
                {
                    index++;
                    if (tokenList[index].CP == "DT")
                    {
                        string CT = tokenList[index].VP;
                        index++;
                        if (tokenList[index].CP == "}")
                        {
                            index++;
                            if (Exp(ref T2,ref Nexp))
                            {
                                if (!(semanticAnalyzer.typeCastCheck(CT, T2)))
                                {
                                    addError("Type Mismatch at " + CT + " " + T2);
                                }
                                if (semanticAnalyzer.comp(T, CT, Op) == "invalid")
                                {
                                    addError("Compatiblity Error");
                                }
                                string TN = icg.CreateTemp();
                                icg.gen(TN + " = " + Nexp);
                                icg.gen(N + " = " + TN);
                                if (LIST(T))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool LIST(string T)
        {
            //FIRST(<LIST >) = {, , ;}
            if (tokenList[index].CP == "," ||
                tokenList[index].CP == ";")
            {
                //<LIST >  , ID <Variable_Link2> | ;
                if (tokenList[index].CP == ",")
                {
                    index++;
                    if (tokenList[index].CP == "ID")
                    {
                        string N = tokenList[index].VP;
                        index++;
                        semanticAnalyzer.insertVariables(N, T, semanticAnalyzer.getCurrentScope());
                        if (Variable_Link2(T,N))
                        {
                            return true;
                        }
                    }
                }
                else if (tokenList[index].CP == ";")
                {
                    index++;
                    return true;
                }
            }
            return false;
        }

        private bool Assign_Op(string T,string N)
        {
            //FIRST(<Assign_Op>) = { = }
            if (tokenList[index].CP == "AOP")
            {
                //<Assign_Op>   = <Assign_Op2>      	
                if (tokenList[index].CP == "AOP")
                {
                    string Eq = tokenList[index].VP;
                    index++;
                    if (Assign_Op2(T,Eq,N))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Assign_Op2(string T,string Eq,string N)
        {
            //FIRST(<Assign_Op2>) = { ID, INT_CONST , FLOAT_CONST , STRING_CONST , CHAR_CONST , BOOL_CONST }
            if (tokenList[index].CP == "ID" ||
                tokenList[index].CP == "num_constant" ||
                tokenList[index].CP == "point_constant" ||
                tokenList[index].CP == "sent_constant" ||
                tokenList[index].CP == "single_constant" ||
                tokenList[index].CP == "bool_constant" ||
                tokenList[index].CP == "{")
            {
                string T1 = "";
                string Nexp = "";
                //<Assign_Op2>  <Exp>;
                if (Exp(ref T1,ref Nexp))
                {
                    string T2 = semanticAnalyzer.comp(T, T1, Eq);
                    if(T2 == "invalid")
                    {
                        addError("Compatiblity Error at"+T+" "+Eq+" "+T1);
                    }
                    string TN = icg.CreateTemp();
                    icg.gen(TN + " = " + Nexp);
                    icg.gen(N + " = " + TN);
                    if (tokenList[index].CP == ";")
                    {
                        index++;
                        return true;
                    }
                }
                else if(tokenList[index].CP == "{")
                {
                    index++;
                    if(tokenList[index].CP == "DT")
                    {
                        string CT = tokenList[index].VP;
                        index++;
                        if(tokenList[index].CP == "}")
                        {
                            index++;
                            string RT = "";
                            if(Exp(ref RT,ref Nexp))
                            {
                                if(!semanticAnalyzer.typeCastCheck(CT,RT))
                                {
                                    addError("Type Mismatch : " + CT + " with " + RT);
                                }
                                string T2 = semanticAnalyzer.comp(T, CT, Eq);
                                if (T2 == "invalid")
                                {
                                    addError("Compatiblity Error at" + T + " " + Eq + " " + CT);
                                }
                                string TN = icg.CreateTemp();
                                icg.gen(TN + " = " + Nexp);
                                icg.gen(N + " = " + TN);
                                if(tokenList[index].CP == ";")
                                {
                                    index++;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool Object_Creation_Exp(string CN,string N,string AM)
        {
            //FIRST(<Object_Creation_Exp>) = {= , , , ;}
            if (tokenList[index].CP == "AOP" ||
                tokenList[index].CP == "," ||
                tokenList[index].CP == ";")
            {
                //<Object_Creation_Exp>  = new ID  (<List_Const>) <Object_List>  |<Object_List>
                if (tokenList[index].CP == "AOP")
                {
                    index++;
                    if (tokenList[index].CP == "new")
                    {
                        index++;
                        if (tokenList[index].CP == "ID")
                        {
                            string CN1 = tokenList[index].VP;
                            if(!(CN == CN1))
                            {
                                addError("Class Mismatch");
                            }
                            index++;
                            if (tokenList[index].CP == "(")
                            {
                                string PL = "";
                                string AL = "";
                                string AQ = "";
                                index++;
                                if (Param_List(ref AL, PL, ref AQ))
                                {
                                    if (tokenList[index].CP == ")")
                                    {
                                        index++;
                                        MethodT mem = new MethodT(CN1, AL, AM, "", true);
                                        if(!semanticAnalyzer.LookUpContructor(mem))
                                        {
                                            addError("Constructor not Found");
                                        }
                                        MethodT mem1 = new MethodT(N, "object", AM, "", false);
                                        if(!semanticAnalyzer.MemberLookUp(mem1))
                                        {
                                            addError("Object Redeclaration");
                                        }
                                        else if(inMethod)
                                        {
                                            semanticAnalyzer.insertVariables(N, CN, semanticAnalyzer.getCurrentScope());
                                        }
                                        else
                                        {
                                            semanticAnalyzer.insertMember(mem1);
                                        }
                                        if (Object_List(CN,AM))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Object_List(CN,AM))
                {
                    return true;
                }
            }
            return false;
        }

        private bool Object_List(string CN,string AM)
        {
            //FIRST(<Object_List>) = {,, ;}
            if (tokenList[index].CP == "," || tokenList[index].CP == ";")
            {
                //<Object_List>  , ID<Object_Creation_Exp>|;
                if (tokenList[index].CP == ",")
                {
                    index++;
                    if (tokenList[index].CP == "ID")
                    {
                        string N = tokenList[index].VP;
                        MethodT mem = new MethodT(N, "object", AM, "", false);
                        if(semanticAnalyzer.MemberLookUp(mem))
                        {
                            addError("Object Redeclaration");
                        }
                        index++;
                        if (Object_Creation_Exp(CN,N,AM))
                        {
                            return true;
                        }
                    }
                }
                else if (tokenList[index].CP == ";")
                {
                    index++;
                    return true;
                }
            }
            return false;
        }

        private bool Object_Call(string T)
        {
            //FIRST(<Object_Call>) = {. , (}
            //FIRST(<Object_Call>) = {. , [}
            if (tokenList[index].CP == "." ||
                tokenList[index].CP == "[")
            {
                //<Object_Call>  . ID <Object_Call>| <Method_Call_1> 
                //<Object_Call> . <Exp> | [<Exp>].<Exp>
                if (tokenList[index].CP == ".")
                {
                    index++;

                    string ET = "";
                    string Nexp = "";
                    if (Exp(ref ET,ref Nexp))
                    {
                      //  Console.Write(NET);
                        return true;
                    }
                }
                else if (tokenList[index].CP == "[")
                {
                    index++;

                    string ET = "";
                    string Nexp = "";
                    if (Exp(ref ET,ref Nexp))
                    {
                        if (tokenList[index].CP == "]")
                        {
                            index++;
                            if (tokenList[index].CP == ".")
                            {
                                index++;
                                string ET2 = "";
                                string Nexp2 = "";
                                if (Exp(ref ET2,ref Nexp2))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool Method_Call(ref string RT,string N)
        {
            //FIRST(<Method_Call_1>) = { ( }
            if (tokenList[index].CP == "(")
            {
                //<Method_Call_1>  (<Param>) ;
                if (tokenList[index].CP == "(")
                {
                    string PL = "";
                    string AL = "";
                    string AQ = "";
                    index++;
                    if (Param_List(ref AL, PL, ref AQ))
                    {
                        if (tokenList[index].CP == ")")
                        {
                            MethodT newMem = new MethodT(N, AL, "", "", true);
                            RT = semanticAnalyzer.SearchMember(semanticAnalyzer.getCurrentClass(), newMem);
                            if(RT == "invalid")
                            {
                                addError("Undeclared Member");
                            }
                            index++;
                            icg.gen("CALL " + semanticAnalyzer.getCurrentClass() +"_"+ N +"_"+AL+","+AQ);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

}
