using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrco.OCL
{
    public class takwin
    {
        //CodeCompliton

        public static Dictionary<string, string> Basic = new Dictionary<string, string>()
        {
            {"out", "out: "},
            {"true", "true"},
            {"false", "false"},
            {"if", "if(){}"},
            {"while", "while(){}"},
            {"new", "new "},
            {"type", "type "},
            {"public", "public "},
            {"private", "private "},
            {"break", "break;"},
        };
        public static List<string> Classes = new List<string>();
        public static List<string> Functions = new List<string>();
        public static List<string> Variables = new List<string>();


        //end
        public static Dictionary<string, string> Langwords = new Dictionary<string, string>()
        {
            {"in", "in"},
            {"out", "out"},
            {"true", "true"},
            {"false", "false"},
            {"if", "if"},
            {"while", "while"},
            {"for", "for"},
            {"new", "new"},
            {"type", "type"},
            {"public", "public"},
            {"private", "private"},
            {"break", "break"},
            {"wait", "wait"},
            {"var", "var"},
            {"ts", "s"},
            {"tm", "m"},
            {"th", "h"},
            {"td", "d"},
        };
        public static void Arabic()
        {
            Langwords = new Dictionary<string, string>()
            {
                { "in", "داخل" },
                { "out", "خارج" },
                { "true", "صحيح" },
                { "false", "خطأ" },
                { "if", "اذا" },
                { "while", "بينما" },
                { "for", "ل" },
                { "new", "جديد" },
                { "type", "نوع" },
                { "public", "عام" },
                { "private", "خاص" },
                { "break", "قطع" },
                { "wait", "انتظر" },
                { "var", "متغير" },
                {"ts", "ث"},
                {"tm", "د"},
                {"th", "س"},
                {"td", "ي"},
            };
            vrcoc.WordReplace = new Dictionary<string, string>
            {
                { "Console", "الوحدة" },
                { "Print", "طباعة" }
            };
        }
        public static void InstallLang()
        {
            //install vars
            vrcoc.Varout.Add(ReadN);
            vrcoc.Varout.Add(CoreGet);
            vrcoc.Varout.Add(IBool);
            vrcoc.Varout.Add(String);
            vrcoc.Varout.Add(Action);
            vrcoc.Varout.Add(Bool);
            vrcoc.Varout.Add(InvokeObject);
            vrcoc.Varout.Add(Number);
            vrcoc.Varout.Add(Link);

            //install basics
            vrcoc.Modes.Add(CorePlay);
            vrcoc.Modes.Add(Wait);
            vrcoc.Modes.Add(ForTime);
            vrcoc.Modes.Add(AddVar);
            vrcoc.Modes.Add(Object); 
            vrcoc.Modes.Add(New);
            vrcoc.Modes.Add(If);
            vrcoc.Modes.Add(Invoke);
            vrcoc.Modes.Add(Type);
            vrcoc.Modes.Add(Break);
        }

        public static void Rest(string on)
        {
            Classes.Distinct();
            Functions.Distinct();
            Variables.Distinct();
        }

        //Core

        public static void CorePlay(string mode)
        {
            if(mode.StartsWith('#')) vrcoc.Cut = mode.Remove(0, 1);
        }
        public static void CoreGet(string mode)
        {
            if (mode.StartsWith('#')) vrcoc.VAR = mode.Remove(0, 1);
        }


        //

        public static void AddVar(string mode)
        {
            if (mode.StartsWith(Langwords["var"]))
            {
                string @object = mode.Remove(0, Langwords["var"].Length + 1);
                string @key;
                if (@object.Contains('='))
                {
                    @key = @object.Split('=')[0];
                    string @value = @object.Split("=")[1];
                    vrcoc.Cut = $"{vrcoc.VarRead(value)}\nS:[Q]OBJECT>:ACTVAR|{key}";
                }
                else
                {
                    @key = @object;
                    vrcoc.Cut = $"V:NULL>\nS:[Q]OBJECT>:ACTVAR|{key}";
                }
                Variables.Add(key);
            }
        }

        public static void IBool(string var)
        {
            if (var[0] == '!')
            {
                vrcoc.VAR = $"{vrcoc.VarRead(var.Remove(0,1))}\nV:[I]BOOL>";
            }
        }

        public static void Break(string var)
        {
            if (var == Langwords["break"]) vrcoc.Cut = "T:BREAK>";
        }

        public static void Wait(string var)
        {
            if (var.StartsWith(Langwords["wait"] + " "))
            {
                string action = var.Remove(0, Langwords["wait"].Length + 1);
                vrcoc.Cut = $"V:STRING>{vrcoc.Compiler(action).Replace("\n", "\\")}\nV:ACTION>\nT:AFTER>";
            }
        }

        public static void ForTime(string var)
        {
            if (var.StartsWith(Langwords["for"] + " "))
            {
                string time = var.Remove(0, Langwords["for"].Length + 1).Remove(var.Remove(0, Langwords["for"].Length + 1).Length - 1);
                string unit = var[var.Length - 1].ToString();
                int multi = 1;
                if (unit == Langwords["ts"]) multi = 1000;
                else if (unit == Langwords["tm"]) multi = 60 * 1000;
                else if (unit == Langwords["th"]) multi = 60 * 60 * 1000;
                else if (unit == Langwords["td"]) multi = 24 * 60 * 60 * 1000;
                vrcoc.Cut = $"V:[32]I>{multi * int.Parse(time)}\nS:[O]GET>:TIMES/FOR\nT:INVOKE>";
            }
        }

        public static async void ReadN(string var)
        {
            if (string.IsNullOrEmpty(var)) vrcoc.VAR = " ";
        }

        public static void Number(string var)
        {
            bool b = false;
            char[] chars = new char[]
            {
                '-',
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9'
            };
            foreach (char c in chars) if (var.StartsWith(c)) b = true;
            if (!b) return;

            bool Multi = var.Contains(',');
            string[] split = var.Split(',');
            if (Multi) var = split[0];

            if(Multi)vrcoc.VAR = vrcoc.VarRead(split[1]);

            string s = "";
            if (var.EndsWith('^'))
            {
                s = "B";
                var = var.Remove(var.Length - 1);
            }
            string number = "0";
            string bytes = "32";
            char mode = 'I';
            if (var.Contains('i') || var.Contains('d'))
            {
                if (var.Contains('i'))
                {
                    mode = 'I';
                    string[] nm = var.Split('i', 2);
                    bytes = nm[1];
                    number = nm[0];
                }
                else
                {
                    mode = 'D';
                    string[] nm = var.Split('d', 2);
                    bytes = nm[1];
                    number = nm[0];
                }
            }
            else
            {
                if (var.Contains('.'))
                {
                    bytes = "4";
                    mode = 'D';
                    number = var;
                }
                else
                {
                    bytes = "32";
                    mode = 'I';
                    number = var;
                }
            }

            vrcoc.VAR += $"\nV:[{s + bytes}]{mode}>{number}";
        }
        public static void Bool(string var)
        {
            if (var.Contains(","))
            {
                string[] split = var.Split(',', 2);
                var = split[0];
                if (var == Langwords["true"]) vrcoc.VAR = vrcoc.VarRead(split[1])+"\nV:BOOL>";
                else if (var == Langwords["false"]) vrcoc.VAR = vrcoc.VarRead(split[1]) + "\nV:BOOL>F";
            }
            else
            {
                if (var == Langwords["true"]) vrcoc.VAR = "V:BOOL>";
                else if (var == Langwords["false"]) vrcoc.VAR = "V:BOOL>F";
            }
        }
        
        public static void New(string var)
        {

            if (var.StartsWith($"{Langwords["new"]} "))
            {
                string @object = var.Remove(0, Langwords["new"].Length+1);
                Classes.Add(@object.Split('(')[1].Remove(@object.Split('(')[1].Length - 1));
                vrcoc.Cut = $"S:NEW>{@object.Split('(')[0]}|{@object.Split('(')[1].Remove(@object.Split('(')[1].Length - 1)}";
            }
        }

        public static void Link(string var)
        {
            //bool v = var.Contains(',');
            //string[] split = var.Split(",", 2);
            //if (v)
            //{
            //    var = split[0];
            //}
            var = var.Replace('.', '/');
            if (var.StartsWith($"{Langwords["out"]}: "))
            {
                var = var.Remove(0, Langwords["out"].Length+2);
                vrcoc.VAR = $"S:[O]GET>{var}";
            }
            else if (var.StartsWith($"{Langwords["in"]}: "))
            {
                var = var.Remove(0, Langwords["in"].Length + 2);
                vrcoc.VAR = $"S:[O]GET>:ACTVAR/{var}";
            }
            else if (!var.Contains('/'))
            {
                vrcoc.VAR = $"S:[L]GET>{var}";
            }
            else
            {
                vrcoc.VAR = $"S:[P]GET>{var}";
            }
            //if (v) vrcoc.VAR = $"{vrcoc.VarRead(split[1])}\n{vrcoc.VAR}";
        }

        public static void If(string var)
        {
            if (var.StartsWith("if("))
            {
                string @object = var.Remove(0, 3);

                string boolen = @object.Split(")", 2)[0];
                string action = @object.Split(")", 2)[1];
                vrcoc.Cut = $"V:STRING>{vrcoc.Compiler(action).Replace("\n", "\\")}\n{vrcoc.VarRead(boolen)}\nT:IF>";
            }
        }

        public static void Type(string var)
        {
            if(var.StartsWith($"{Langwords["type"]} "))
            {
                string @object = var.Remove(0, Langwords["type"].Length + 1);
                Classes.Add(@object);
                vrcoc.Cut = $"S:[S]TYPE>{@object}";
            }
        }
        
        public static void Object(string var)
        {
            if (var.StartsWith($"{Langwords["public"]} "))
            {
                string @object = var.Remove(0, Langwords["public"].Length+1);
                string @key = @object.Split('=')[0];
                string @value = @object.Remove(0, key.Length + 1);
                Variables.Add(key);
                vrcoc.Cut = $"{vrcoc.VarRead(value)}\nS:[U]OBJECT>{key}";
            }
            else if (var.StartsWith($"{Langwords["private"]} "))
            {
                string @object = var.Remove(0, Langwords["private"].Length + 1);
                string @key = @object.Split('=')[0];
                Variables.Add(key);
                string @value = @object.Remove(0, key.Length + 1);
                vrcoc.Cut = $"{vrcoc.VarRead(value)}\nS:[R]OBJECT>{key}";
            }
        }

        public static void String(string var)
        {
            if (var.StartsWith('\"'))
            {
                if (var.Contains("\","))
                {
                    var = var.Remove(0, 1);
                    string[] split = var.Split("\",", 2);
                    var = split[0];
                    vrcoc.VAR = $"{vrcoc.VarRead(split[1])}\nV:STRING>{var}";
                }
                else vrcoc.VAR = $"V:STRING>{var.Remove(0, 1).Remove(var.Length - 2, 1)}";
            }
        }

        public static void Action(string line)
        {
            if (line.StartsWith("(") && line.Contains("){") && line.EndsWith("}"))
            {
                line = line.Remove(0, 1);
                line = line.Remove(line.Length - 1);
                string var = line.Split("){")[0];
                string act = line.Remove(0, var.Length + 2);
                vrcoc.VAR = $"V:STRING>{vrcoc.Compiler(act).Replace('\n', '\\')}\nV:ACTION>{var.Replace(" ", "").Replace(',', '|')}";
            }
        }
        public static void InvokeObject(string line)
        {
            string[] split = line.Split(",", 2);
            bool multi = line.Contains("),");
            if (multi) line = split[0];
            bool OUT = line.StartsWith($"{Langwords["out"]}: ");

            if (OUT) line = line.Remove(0, Langwords["out"].Length+2);
            if (line.Contains('(') && line.EndsWith(")"))
            {
                string Path = line.Split('(', 2)[0];
                string Var = line.Split('(', 2)[1];
                Var = Var.Remove(Var.Length - 1, 1);

                if (!OUT)
                    vrcoc.VAR = $"{vrcoc.VarRead(Var)}\nS:[P]GET>{Path.Replace('.', '/')}\nT:INVOKE>";
                else
                    vrcoc.VAR = $"{vrcoc.VarRead(Var)}\nS:[O]GET>{Path.Replace('.', '/')}\nT:INVOKE>";
            }
            if (multi) vrcoc.VAR = $"{vrcoc.VarRead(split[1])}\n{vrcoc.VAR}";
        }
        public static void Invoke(string line)
        {
            bool OUT = line.StartsWith($"{Langwords["out"]}: ");
            if (OUT) line = line.Remove(0, Langwords["out"].Length+2);
            if (line.Contains('(') && line.EndsWith(")"))
            {
                string Path = line.Split('(')[0];
                string Var = line.Remove(0, Path.Length + 1);
                Var = Var.Remove(Var.Length - 1, 1);

                string comp = "";
                if (Var != "") comp = $"{vrcoc.VarRead(Var)}\n";
                if (OUT) vrcoc.Cut = $"{comp}S:[O]GET>{Path.Replace('.', '/')}\nT:INVOKE>";
                else if(Path.Contains('.')) vrcoc.Cut = $"{comp}S:[P]GET>{Path.Replace('.', '/')}\nT:INVOKE>";
                else vrcoc.Cut = $"{comp}S:[L]GET>{Path.Replace('.', '/')}\nT:INVOKE>";
            }
        }
    }
}
