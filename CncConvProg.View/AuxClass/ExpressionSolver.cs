using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Reflection;
using Microsoft.JScript;

namespace CncConvProg.View.AuxClass
{
    class ExpressionEvaluator
    {



        public static double EvalToDouble(string statement)
        {
            string s = EvalToString(statement);
            return double.Parse(s);
        }

        public static string EvalToString(string statement)
        {
            if (NumberFormatInfo.CurrentInfo.NumberDecimalSeparator == ",")
            {
                statement = statement.Replace(",", ".");
            }

            object o = EvalToObject(statement);

            //if (NumberFormatInfo.CurrentInfo.NumberDecimalSeparator != ".")
            //{
            //    o = o.ToString().Replace(".", ",");
            //}
            var o1 = o.ToString();

            double flt = double.Parse(o1, CultureInfo.InvariantCulture.NumberFormat);
            var rslt = flt.ToString(CultureInfo.InvariantCulture.NumberFormat);

            if (NumberFormatInfo.CurrentInfo.NumberDecimalSeparator == ",")
            {
                rslt = rslt.Replace(".", ",");
            }

            return rslt;


        }

        public static object EvalToObject(string statement)
        {
            return _evaluatorType.InvokeMember(
                        "Eval",
                        BindingFlags.InvokeMethod,
                        null,
                        _evaluator,
                        new object[] { statement }
                     );
        }

        static ExpressionEvaluator()
        {
            ICodeCompiler compiler;
            compiler = new JScriptCodeProvider().CreateCompiler();

            CompilerParameters parameters;
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            CompilerResults results;
            results = compiler.CompileAssemblyFromSource(parameters, _jscriptSource);
            Assembly assembly = results.CompiledAssembly;
            _evaluatorType = assembly.GetType("Evaluator.Evaluator");

            _evaluator = Activator.CreateInstance(_evaluatorType);
        }

        private static object _evaluator = null;
        private static Type _evaluatorType = null;
        private static readonly string _jscriptSource =

            @"package Evaluator
            {
               class Evaluator
               {
                  public function Eval(expr : String) : String 
                  { 
                     return eval(expr); 
                  }
               }
            }";
    }
}
