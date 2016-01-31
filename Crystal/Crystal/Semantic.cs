using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal
{
    class Semantic
    {
        public static List<String> errorlist = new List<string>();
        
        public static List<ClassT> ClassSymbolTable = new List<ClassT>();
        public static globaldata MainSymbolTable;

        Stack<int> Scope = new Stack<int>();
        int currentScope = 0;

        public Semantic()
        {
            Semantic.MainSymbolTable = new globaldata();
        }

        //Compatiblities
        public bool Comp(string type,string OP)
        {
            if (OP == "INC_DEC")
            {
                switch (type)
                {
                    case "number":
                    case "number_constant":
                    case "point":
                    case "point_constant":
                    case "single":
                    case "single_constant":
                        return true;
                    default:
                        return false;
                }
            }
            else if(OP == "||" || OP == "&&")
            {
                switch (type)
                {
                    case "bool_constant":
                    case "true":
                    case "false":
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
        
        public bool CCheckReturnType(string name,string type)
        {
            for (int i = 0; i < MainSymbolTable.classes.Count;i++ )
            {
                for (int j = 0; j < MainSymbolTable.classes[i].classdata.Count; j++)
                {
                    if (MainSymbolTable.classes[i].classdata[j].name == name)
                    {
                        string ParamWithRT = MainSymbolTable.classes[i].classdata[j].type;
                        string[] param = ParamWithRT.Split('>');
                        string RT = param[1];
                        if(type == RT || (type == "point" && RT == "number"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public string comp(string t1, string t2, string op)
        {
            string RT = "";

            if (op == "+" || op == "-" || op == "*" || op == "/")
            {
                switch (t1)
                {
                    case "number":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "number";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "point";
                        else if ((t2 == "sent" || t2 == "sent_constant") && op == "+")
                            RT = "sent";
                        else if (t2 == "single" || t2 == "single_constant")
                            RT = "number";
                        else if (t2 == "bool" || t2 == "bool_constant")
                            RT = "invalid";
                        else RT = "invalid";
                        break;
                    case "point":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "point";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "point";
                        else if ((t2 == "sent" || t2 == "sent_constant") && op == "+")
                            RT = "sent";
                        else if (t2 == "single" || t2 == "single_constant")
                            RT = "point";
                        else if (t2 == "bool" || t2 == "bool_constant")
                            RT = "invalid";
                        else RT = "invalid";
                        break;
                    case "sent":
                        if ((t2 == "number" || t2 == "number_constant") && op == "+")
                            RT = "sent";
                        else if ((t2 == "point" || t2 == "point_constant") && op == "+")
                            RT = "sent";
                        else if ((t2 == "sent" || t2 == "sent_constant") && op == "+")
                            RT = "sent";
                        else if ((t2 == "single" || t2 == "sent_constant") && op == "+")
                            RT = "sent";
                        else if ((t2 == "bool" || t2 == "bool_constant") && op == "+")
                            RT = "sent";
                        else RT = "invalid";
                        break;
                    case "single":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "number";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "point";
                        else if ((t2 == "sent" || t2 == "sent_constant") && op == "+")
                            RT = "sent";
                        else if (t2 == "single" || t2 == "single_constant")
                            RT = "number";
                        else if (t2 == "bool" || t2 == "bool_constant")
                            RT = "invalid";
                        else RT = "invalid";
                        break;
                    case "bool":
                        if ((t2 == "sent" || t2 == "sent_constant") && op == "+")
                            RT = "sent";
                        else RT = "invalid";
                        break;
                }
            }
            else if (op == ">" || op == ">=" || op == "<" || op == "<=" || op == "&&" || op == "||" || op == "==")
            {
                switch (t1)
                {
                    case "number":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "bool";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "bool";
                        else if (t2 == "sent" || t2 == "sent_constant")
                            RT = "invalid";
                        else if (t2 == "single" || t2 == "single_constant")
                            RT = "bool";
                        else if (t2 == "bool" || t2 == "bool_constant")
                            RT = "invalid";
                        else RT = "invalid";
                        break;
                    case "point":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "bool";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "bool";
                        else if (t2 == "sent" || t2 == "sent_constant")
                            RT = "invalid";
                        else if (t2 == "single" || t2 == "single_constant")
                            RT = "bool";
                        else if (t2 == "bool" || t2 == "bool_constant")
                            RT = "invalid";
                        else RT = "invalid";
                        break;
                    case "sent":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "invalid";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "invalid";
                        else if (t2 == "sent" || t2 == "sent_constant")
                            RT = "invalid";
                        else if (t2 == "single" || t2 == "single_constant")
                            RT = "invalid";
                        else if (t2 == "bool" || t2 == "bool_constant")
                            RT = "invalid";
                        else RT = "invalid";
                        break;
                    case "single":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "bool";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "bool";
                        else if (t2 == "sent" || t2 == "sent_constant")
                            RT = "invalid";
                        else if (t2 == "single" || t2 == "single_constant")
                            RT = "bool";
                        else if (t2 == "bool" || t2 == "bool_constant")
                            RT = "invalid";
                        else RT = "invalid";
                        break;
                    case "bool":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "invalid";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "invalid";
                        else if (t2 == "sent" || t2 == "sent_constant")
                            RT = "invalid";
                        else if (t2 == "single" || t2 == "single_constant")
                            RT = "invalid";
                        else if (t2 == "bool" && (op == "&&" || op == "||"))
                            RT = "bool";
                        else RT = "invalid";
                        break;
                }
            }
            else if (op == "=" || op == "+=" || op == "-=")
            {
                switch (t1)
                {
                    case "number":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "number";
                        else if (t2 == "single" || t2 == "single_constant")
                            RT = "number";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "number";
                        else RT = "invalid";
                        break;
                    case "point":
                        if (t2 == "number" || t2 == "number_constant")
                            RT = "point";
                        else if (t2 == "point" || t2 == "point_constant")
                            RT = "point";
                        else RT = "invalid";
                        break;
                    case "sent":
                        if ((t2 == "number" || t2 == "number_constant") && op == "+=")
                            RT = "sent";
                        else if ((t2 == "point" || t2 == "point_constant") && op == "+=")
                            RT = "sent";
                        else if ((t2 == "sent" || t2 == "sent_constant") && (op == "+=" || op == "="))
                            RT = "sent";
                        else if ((t2 == "single" || t2 == "single_constant") && op == "+=")
                            RT = "sent";
                        else RT = "invalid";
                        break;
                    case "single":
                        if (t2 == "single" || t2 == "single_constant")
                            RT = "single";
                        else RT = "invalid";
                        break;
                    case "bool":
                        if (t2 == "bool" || t2 == "bool_constant")
                            RT = "bool";
                        else RT = "invalid";
                        break;
                }
            }
            return RT;
        }

        //Insert Functions
        public bool insertClass(string N, string AM, string P)
        {
            ClassT currentClass = new ClassT(N, AM, P);

            if (!ClassLookUp(currentClass.name))
            {
                MainSymbolTable.classes.Add(currentClass);
                return true;
            }
            else
            {
                errorlist.Add("Class Redeclaration Error of " + N);
            }
            return false;
        }

        public bool insertConstructor(MethodT obj)
        {
            if (obj.name == MainSymbolTable.classes.Last().name)
            {
                return insertMember(obj);
            }
            else
            {
                errorlist.Add("Constructor Redeclaration " + obj.name + " " + obj.type);
                return false;
            }
        }

        public bool insertMember(MethodT obj)
        {
            MethodT currentMember = (MethodT)obj.shallowCopy();
            if (MemberLookUp(currentMember))
            {
                errorlist.Add("Method Redeclaration Error " + currentMember.name + " "  + currentMember.type);
                return false;
            }
            else
            {
                MainSymbolTable.classes.Last().classdata.Add(currentMember);
                return true;
            }

        }

        public bool insertVariables(string N, string T, int S)
        {
            VariableData currentVariable = new VariableData(N, T, S);
            if (!(MethodVariableLookUp(currentVariable.name, currentVariable.scope)))
            {
                MainSymbolTable.classes.Last().classdata.Last().var.Add(currentVariable);
                return true;
            }
            else
            {
                errorlist.Add("Variable Redeclaration " + N);
                return false;
            }
        }

        //Lookups
        public bool ClassLookUp(String N)
        {
            for (int i = 0; i < MainSymbolTable.classes.Count; i++)
            {
                if (MainSymbolTable.classes[i].name == N)
                {
                    return true;
                }
            }
            return false;
        }

        public bool MethodVariableLookUp(String N, int S)
        {
            if (MainSymbolTable.classes.Last().classdata.Count > 0)
            {
                for (int i = 0; i < MainSymbolTable.classes.Last().classdata.Last().var.Count; i++)
                {
                    if (MainSymbolTable.classes.Last().classdata.Last().var[i].scope == S &&
                        MainSymbolTable.classes.Last().classdata.Last().var[i].name == N)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public bool MemberLookUp(MethodT obj)
        {
            //for methods
            bool paramNotMatch = false;
            if (obj.methodFlag)
            {
                for (int i = 0; i < MainSymbolTable.classes.Last().classdata.Count; i++)
                {
                    if (MainSymbolTable.classes.Last().classdata[i].methodFlag &&
                        MainSymbolTable.classes.Last().classdata[i].name == obj.name)
                    {
                        string ParamWithRT = MainSymbolTable.classes.Last().classdata[i].type;
                        string[] Param = ParamWithRT.Split('>');
                        string currentParam = Param[0];
                        ParamWithRT = obj.type;
                        Param = ParamWithRT.Split('>');
                        string objParam = Param[0];
                        if (objParam == currentParam)
                        {
                            paramNotMatch = true;
                        }
                        else
                        {
                            paramNotMatch = false;
                            break;
                        }
                    }
                }

                if (paramNotMatch)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //for fields
            else
            {
                for (int i = 0; i < MainSymbolTable.classes.Last().classdata.Count; i++)
                {
                    if (!MainSymbolTable.classes.Last().classdata[i].methodFlag &&
                        MainSymbolTable.classes.Last().classdata[i].name == obj.name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool LookUpContructor(MethodT obj)
        {
            for (int j = 0; j < MainSymbolTable.classes.Count; j++)
            {
                if (MainSymbolTable.classes[j].name == obj.name)
                {
                    foreach (MethodT i in MainSymbolTable.classes[j].classdata)
                    {
                        if (obj.name == i.name)
                        {
                            if (obj.type == i.type)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        //Scope Functions
        public void createScope()
        {
            Scope.Push(currentScope++);
        }

        public void deleteScope()
        {
            Scope.Pop();
        }

        public int getCurrentScope()
        {
            return Scope.Peek();
        }

        public string getCurrentClass()
        {
            return MainSymbolTable.classes.Last().name;
        }

        public bool Search(string name)
        {
            int[] scopeArray = Scope.ToArray();
            for (int i = scopeArray.Count() - 1; i >= 0; i--)
            {
                
                if (MethodVariableLookUp(name, scopeArray[i]))
                {
                    return true;
                }
            }

            for (int i = 0; i < MainSymbolTable.classes.Last().classdata.Count; i++)
            {
                if (!MainSymbolTable.classes.Last().classdata[i].methodFlag &&
                    MainSymbolTable.classes.Last().classdata[i].name == name)
                {
                    return true;
                }
            }
            return false;
        }

         public bool SearchMember(string classname, ref MethodT obj)
        {
            // a.b;   || a.b(int i, float j); || a.b(int j)
            // classType.obj
            for (int i = 0; i < MainSymbolTable.classes.Count; i++)
            {

                if (MainSymbolTable.classes[i].name == classname)
                {
                    ClassT currentClass = MainSymbolTable.classes[i];
                    for (int j = 0; j < currentClass.classdata.Count; j++)
                    {
                        if (currentClass.classdata[j].name == obj.name)
                        {
                            MethodT currentMember = currentClass.classdata[j];
                            if (!obj.methodFlag && !currentMember.methodFlag) // check for variable of a class
                            {
                                obj.type = currentMember.type;
                                return true;
                            }

                            else if (obj.methodFlag && currentMember.methodFlag) // check for method of a class
                            {
                                string paramWithRT = currentMember.type;
                                string[] param = paramWithRT.Split('>');
                                string RT = param[1];
                                string paramAL = param[0];
                                paramWithRT = obj.type;
                                param = paramWithRT.Split('>');
                                string objParam = param[0];
                                if (objParam == paramAL)
                                {
                                    obj.type = currentMember.type;
                                    return true;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    } 
                    return false;
                }
            }  
            return false;
        }

        public string SearchMember(string classname, MethodT obj)
        {
            // classname.obj
            // a.b;   || a.b(int i, float j);
            for (int i = 0; i < MainSymbolTable.classes.Count; i++)
            {

                if (MainSymbolTable.classes[i].name == classname)
                {
                    ClassT currentClass = MainSymbolTable.classes[i];
                    
                    for (int j = 0; j < currentClass.classdata.Count; j++)
                    {
                        if (currentClass.classdata[j].name == obj.name)
                        {
                            MethodT currentMember = currentClass.classdata[j];
                            if (!obj.methodFlag && !currentMember.methodFlag) // check for variable of a class
                            {
                                obj.type = currentMember.type;
                                return obj.type;
                            }
                            else if (obj.methodFlag && currentMember.methodFlag) // check for method of a class
                            {
                                string paramWithRT = currentMember.type;
                                string[] param = paramWithRT.Split('>');
                                string RT = param[1];
                                string paramAL = param[0];
                                paramWithRT = obj.type;
                                param = paramWithRT.Split('>');
                                string objParam = param[0];
                                if (objParam == paramAL)
                                {
                                    obj.type = currentMember.type;
                                    return obj.type;
                                }
                            }
                            else
                            {
                                return "invalid";
                            }
                        }
                    } // For loop end for finding member of a class
                    return "invalid";
                }
            } // For loop end for finding classes 
            return "invalid";
        }

        //get type for compatiblity
        public string getType(string name)
        {
            //for variable within method
            int[] scopeArray = Scope.ToArray();
            for (int j = scopeArray.Count() - 1; j >= 0; j--)
            {
                if (MainSymbolTable.classes.Last().classdata.Count > 0)
                {
                    for (int i = 0; i < MainSymbolTable.classes.Last().classdata.Last().var.Count; i++)
                    {
                        if (MainSymbolTable.classes.Last().classdata.Last().var[i].name == name)
                        {
                            return MainSymbolTable.classes.Last().classdata.Last().var[i].type;
                        }
                    }
                }
            }
            //for variable within class
            for (int i = 0; i < MainSymbolTable.classes.Last().classdata.Count; i++)
            {
                if (!MainSymbolTable.classes.Last().classdata[i].methodFlag &&
                    MainSymbolTable.classes.Last().classdata[i].name == name)
                {
                    return MainSymbolTable.classes.Last().classdata[i].type;
                }
            }
            //for method return type
            for (int i = 0; i < MainSymbolTable.classes.Last().classdata.Count; i++)
            {
                if (MainSymbolTable.classes.Last().classdata[i].methodFlag &&
                    MainSymbolTable.classes.Last().classdata[i].name == name)
                {
                    string paramWithRT = MainSymbolTable.classes.Last().classdata[i].type;
                    string[] param = paramWithRT.Split('>');
                    string RT = param[0];
                    return RT;
                }
            }
            return "invalid";
        }
        public bool typeCastCheck(string t1,string t2)
        {
            
            switch (t1)
            {
                case "number":
                    if (t2 == "number")
                        return true;
                    else if (t2 == "point" || t2 == "point_constant")
                        return true;
                    else if (t2 == "sent" || t2 == "sent_constant")
                        return false;
                    else if ((t2 == "single" || t2 == "single_constant") && t2 != "")
                        return true;
                    else if (t2 == "bool" || t2 == "bool_constant")
                        return false;
                    else return false;
                    
                case "point":
                    if (t2 == "number" || t2 == "number_constant")
                        return true;
                    else if (t2 == "point" || t2 == "point_constant")
                        return true;
                    else if (t2 == "sent" || t2 == "sent_constant")
                        return false;
                    else if (t2 == "single" || t2 == "single_constant")
                        return true;
                    else if (t2 == "bool" || t2 == "bool_constant")
                        return false;
                    else return false;
                    
                case "sent":
                    return false;
                    
                case "single":
                    if (t2 == "number" || t2 == "number_constant")
                        return true;
                    else if (t2 == "point" || t2 == "point_constant")
                        return true;
                    else if (t2 == "sent" || t2 == "sent_constant")
                        return false;
                    else if (t2 == "single" || t2 == "single_constant")
                        return true;
                    else if (t2 == "bool" || t2 == "bool_constant")
                        return false;
                    else return false;
                    
                case "bool":
                    if (t2 == "bool" || t2 == "bool_constant")
                        return true;
                    else return false;
                default:
                    return false;
            }
        }

    }



    class globaldata
    {
        public List<ClassT> classes;
        public List<globaldata> globalVariables;

        public globaldata()
        {
            classes = new List<ClassT>();
            globalVariables = new List<globaldata>();
        }
        public globaldata ShallowCopy()
        {
            return (globaldata)this.MemberwiseClone();
        }
    }
    class ClassT
    {
        public string name;
        public string type = "class";
        public string AM;
        public string Parent;
        public List<MethodT> classdata = new List<MethodT>();

        public ClassT(string n, string am, string parent)
        {
            this.name = n;
            this.AM = am;
            this.Parent = parent;
        }

        public ClassT shallowCopy()
        {
            return (ClassT)this.MemberwiseClone();
        }
    }
    class MethodT
    {
        public string name;
        public string type;
        public string AM;
        public string TM;
        public bool methodFlag;
        public List<VariableData> var;

        public MethodT(string n, string t, string am, string tm,bool mflag)
        {
            this.name = n;
            this.type = t;
            this.AM = am;
            this.TM = tm;
            this.methodFlag = mflag;
            if (methodFlag)
            {
                var = new List<VariableData>();
            }
        }

        public MethodT shallowCopy()
        {
            return (MethodT)this.MemberwiseClone();
        }
    }
    class VariableData
    {
        public string name;
        public string type;
        public int scope;
        public VariableData(string n, string t, int s)
        {
            this.name = n;
            this.type = t;
            this.scope = s;
        }

        public VariableData shallowCopy()
        {
            return (VariableData)this.MemberwiseClone();
        }
    }
}
