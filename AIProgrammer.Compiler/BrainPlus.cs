﻿using AIProgrammer.Types.Interface;
using AIProgrammer.Fitness;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AIProgrammer.Compiler
{
    /// <summary>
    /// Usage: Compiler.Compile("++++.", "StringFitness", 2000);
    /// Output: exe file thats runs the brainfuck code.
    /// </summary>
    public static class BrainPlus
    {
        /// <summary>
        /// Compiles brainfuck code into an executable.
        /// </summary>
        /// <param name="program">Brainfuck source code</param>
        /// <param name="pathName">Executable file path</param>
        /// <param name="fitness">IFitness</param>
        /// <param name="includeHeader">True to display the header (Brainfuck .NET Compiler 1.0, Created by ...).</param>
        public static void Compile(string program, string pathName, IFitness fitness, bool includeHeader = true)
        {
            Compile(program, pathName, fitness.GetType().Name, fitness.GetConstructorParameters());
        }

        /// <summary>
        /// Compiles brainfuck code into an executable.
        /// </summary>
        /// <param name="program">Brainfuck source code</param>
        /// <param name="pathName">Executable file path</param>
        /// <param name="fitnessMethod">string (HelloUserFitness, StringOptimizedFitness, etc)</param>
        /// <param name="constructorParams">Parameters to pass to fitness method constructor, after GA.</param>
        /// <param name="includeHeader">True to display the header (Brainfuck .NET Compiler 1.0, Created by ...).</param>
        public static void Compile(string program, string pathName, string fitnessMethod, string constructorParams, bool includeHeader = true)
        {
            string sourceCode = @"
using System;
using AIProgrammer.Fitness.Concrete;
using AIProgrammer.Types.Interface;

class Program {
    public static void Main(string[] args)
    {
        string program = ""[SOURCE]"";
        IFitness fitness = new [FITNESSMETHOD](null, [PARAMETERS]);

        // Specify -s to supress header text.
        if (args.Length == 0 || args[0] != ""-s"")
        {
            [HEADER]
        }

        Console.WriteLine(fitness.RunProgram(program));

        Console.WriteLine();
        if (args.Length == 0 || args[0] != ""-s"")
        {
            Console.Write(""Press any key to continue.."");
            Console.ReadKey();
        }
    }
}";

            CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            CompilerParameters parameters = new CompilerParameters(new [] { "mscorlib.dll", "System.Core.dll" }, pathName, true);
            parameters.GenerateExecutable = true;
            parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(AIProgrammer.Fitness.Concrete.StringFitness)).Location);
            parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(AIProgrammer.Types.Interface.IFitness)).Location);
            parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(AIProgrammer.GeneticAlgorithm.GA)).Location);

            if (includeHeader)
            {
                sourceCode = sourceCode.Replace("[HEADER]", @"
                    Console.WriteLine(""BrainPlus Compiler 1.0"");
                    Console.WriteLine(""Created by Kory Becker"");
                    Console.WriteLine(""http://www.primaryobjects.com/kory-becker.aspx"");
                    Console.WriteLine(""Running program:"");
                    Console.WriteLine(""" + program + @""");
                    Console.WriteLine();");
            }
            else
            {
                sourceCode = sourceCode.Replace("[HEADER]", "");
            }

            sourceCode = sourceCode.Replace("[SOURCE]", program);
            sourceCode = sourceCode.Replace("[FITNESSMETHOD]", fitnessMethod);
            sourceCode = sourceCode.Replace("[PARAMETERS]", constructorParams);

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, sourceCode);

            results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
        }
    }
}
