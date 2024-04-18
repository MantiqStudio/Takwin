using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OebCode.OCL
{
    public class takwin
    {
        public static Dictionary<string, string> Langwords = new Dictionary<string, string>()
        {
            {"out", "out"},
            {"true", "true"},
            {"false", "false"},
            {"if", "if"},
            {"while", "while"},
            {"new", "new"},
            {"type", "type"},
            {"public", "public"},
            {"private", "private"},
        };
        public static void Arabic()
        {
            Langwords = new Dictionary<string, string>()
            {
                { "out", "خارج" },
                { "true", "صحيح" },
                { "false", "خطأ" },
                { "if", "اذا" },
                { "while", "بينما" },
                { "new", "جديد" },
                { "type", "نوع" },
                { "public", "عام" },
                { "private", "خاص" },
            };
            OebCodeCompiler.WordReplace = new Dictionary<string, string>
            {
                { "Console", "الوحدة" },
                { "Print", "طباعة" }
            };
        }
        public static void InstallLang()
        {
            //install vars
            OebCodeCompiler.Varout.Add(ReadN);
            OebCodeCompiler.Varout.Add(String);
            OebCodeCompiler.Varout.Add(Bool);
            OebCodeCompiler.Varout.Add(InvokeObject);
            OebCodeCompiler.Varout.Add(Number);
            OebCodeCompiler.Varout.Add(Link);

            //install basics
            OebCodeCompiler.Modes.Add(New);
            OebCodeCompiler.Modes.Add(If);
            OebCodeCompiler.Modes.Add(Invoke);
            OebCodeCompiler.Modes.Add(Object);
            OebCodeCompiler.Modes.Add(Action);
            OebCodeCompiler.Modes.Add(Type);
        }

        public static async void ReadN(string var)
        {
            if (string.IsNullOrEmpty(var)) OebCodeCompiler.VAR = " ";
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
            foreach (char c in chars) if (var.Contains(c)) b = true;
            if (!b) return;

            bool Multi = var.Contains(',');
            string[] split = var.Split(',');
            if (Multi) var = split[0];

            if(Multi)OebCodeCompiler.VAR = OebCodeCompiler.VarRead(split[1]);

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

            OebCodeCompiler.VAR += $"\nV:[{s + bytes}]{mode}>{number}";
        }
        public static void Bool(string var)
        {
            if (var.Contains(","))
            {
                string[] split = var.Split(',', 2);
                var = split[0];
                if (var == Langwords["true"]) OebCodeCompiler.VAR = OebCodeCompiler.VarRead(split[1])+"\nV:BOOL>";
                else if (var == Langwords["false"]) OebCodeCompiler.VAR = OebCodeCompiler.VarRead(split[1]) + "\nV:BOOL>F";
            }
            else
            {
                if (var == Langwords["true"]) OebCodeCompiler.VAR = "V:BOOL>";
                else if (var == Langwords["false"]) OebCodeCompiler.VAR = "V:BOOL>F";
            }
        }
        
        public static void New(string var)
        {

            if (var.StartsWith($"{Langwords["new"]} "))
            {
                string @object = var.Remove(0, Langwords["new"].Length+1);
                OebCodeCompiler.Cut = $"S:NEW>{@object.Split('(')[0]}|{@object.Split('(')[1].Remove(@object.Split('(')[1].Length - 1)}";
            }
        }

        public static void Link(string var)
        {
            bool v = var.Contains(',');
            string[] split = var.Split(",", 2);
            if (v)
            {
                var = split[0];
            }
            var = var.Replace('.', '/');
            if (var.StartsWith($"{Langwords["out"]}: "))
            {
                var = var.Remove(0, Langwords["out"].Length+2);
                OebCodeCompiler.VAR = $"S:[O]GET>{var}";
            }
            else if (!var.Contains('.'))
            {
                OebCodeCompiler.VAR = $"S:[L]GET>{var}";
            }
            else
            {
                OebCodeCompiler.VAR = $"S:[P]GET>{var}";
            }
            if (v) OebCodeCompiler.VAR = $"{OebCodeCompiler.VarRead(split[1])}\n{OebCodeCompiler.VAR}";
        }

        public static void If(string var)
        {
            if (var.StartsWith("if("))
            {
                string @object = var.Remove(0, 3);

                string boolen = @object.Split(")", 2)[0];
                string action = @object.Split(")", 2)[1];
                OebCodeCompiler.Cut = $"V:STRING>{OebCodeCompiler.Compiler(action).Replace("\n", "\\")}\n{OebCodeCompiler.VarRead(boolen)}\nT:IF>";
            }
        }

        public static void Type(string var)
        {
            if(var.StartsWith($"{Langwords["type"]} "))
            {
                string @object = var.Remove(0, Langwords["type"].Length + 1);
                OebCodeCompiler.Cut = $"S:[S]TYPE>{@object}";
            }
        }
        
        public static void Object(string var)
        {
            if(var.StartsWith($"{Langwords["public"]} "))
            {
                string @object = var.Remove(0, Langwords["public"].Length+1);
                string @key = @object.Split('=')[0];
                string @value = @object.Split("=")[1];
                OebCodeCompiler.Cut = $"{OebCodeCompiler.VarRead(value)}\nS:[U]OBJECT>{key}";
            }
            else if (var.StartsWith($"{Langwords["private"]} "))
            {
                string @object = var.Remove(0, Langwords["private"].Length + 1);
                string @key = @object.Split('=')[0];
                string @value = @object.Split("=")[1];
                OebCodeCompiler.Cut = $"{OebCodeCompiler.VarRead(value)}\nS:[R]OBJECT>{key}";
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
                    OebCodeCompiler.VAR = $"{OebCodeCompiler.VarRead(split[1])}\nV:STRING>{var}";
                }
                else OebCodeCompiler.VAR = $"V:STRING>{var.Remove(0, 1).Remove(var.Length - 2, 1)}";
            }
        }

        public static void Action(string line)
        {
            if (line.StartsWith("(") && line.Contains("){") && line.EndsWith("}"))
            {
                line = line.Remove(0, 1);
                line = line.Remove(line.Length - 1);
                string var = line.Split("){")[0];
                string act = line.Remove(0, line.Length + 2);
                OebCodeCompiler.Cut = $"V:STRING>{OebCodeCompiler.Compiler(act).Replace('\n', '\\')}V:ACTION>{var.Replace(" ", "").Replace(',', '|')}";
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

                if (!OUT) OebCodeCompiler.VAR = $"{OebCodeCompiler.VarRead(Var)}\nS:[P]GET>{Path.Replace('.', '/')}\nT:INVOKE>";
                else OebCodeCompiler.VAR = $"{OebCodeCompiler.VarRead(Var)}\nS:[O]GET>{Path.Replace('.', '/')}\nT:INVOKE>";
            }
            if (multi) OebCodeCompiler.VAR = $"{OebCodeCompiler.VarRead(split[1])}\n{OebCodeCompiler.VAR}";
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
                
                if (!OUT) OebCodeCompiler.Cut = $"{OebCodeCompiler.VarRead(Var)}\nL:[O]GET>{Path.Replace('.', '/')}\nT:INVOKE>";
                else OebCodeCompiler.Cut = $"{OebCodeCompiler.VarRead(Var)}\nS:[O]GET>{Path.Replace('.', '/')}\nT:INVOKE>";
            }
        }
    }
}
