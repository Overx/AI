﻿using AIProgrammer.Fitness.Base;
using AIProgrammer.GeneticAlgorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AIProgrammer.Fitness.Concrete
{
    public class XmlToJsonFitness : FitnessBase
    {
        private static string[] _trainingExamples = { "<a>boy</a>", "<p>cat</p>" };
        private static string[] _trainingResults = new string[_trainingExamples.Length];

        /// <summary>
        /// Previously generated BrainPlus functions for outputting json characters: { } " :
        /// To use, set _appendCode = XmlToJsonFitness.Function in main program.
        /// 
        /// Generated using StrictStringFitness with StringFunction with the following settings:
        /// TargetString = "{ } \" :"
        /// private static IFunction _functionGenerator = new StringFunction(() => GetFitnessMethod(), _bestStatus, fitnessFunction, OnGeneration, _crossoverRate, _mutationRate, _genomeSize, _targetParams);
        /// ...
        /// return new StringStrictFitness(_ga, _maxIterationCount, _targetParams.TargetString, _appendCode);
        /// </summary>
        public static string Function = "8-----.@-[8[[---.@D+2++.@->4------.@";

        public XmlToJsonFitness(GA ga, int maxIterationCount, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            if (_targetFitness == 0)
            {
                for (int i = 0; i < _trainingExamples.Length; i++)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_trainingExamples[i]);
                    string json = JsonConvert.SerializeXmlNode(doc);

                    _trainingResults[i] = json;
                    _targetFitness += json.Length * 256;
                }
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            double countBonus = 0;
            double penalty = 0;

            for (int i = 0; i < _trainingExamples.Length; i++)
            {
                try
                {
                    int state = 0;
                    _console.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state < _trainingExamples[i].Length)
                        {
                            // Send input.
                            return (byte)_trainingExamples[i][state++];
                        }
                        else
                        {
                            // Not ready for input.
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        _console.Append((char)b);
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                _output.Append(_console.ToString());
                _output.Append("|");

                // Check result.
                for (int j = 0; j < _trainingResults[i].Length; j++)
                {
                    if (_console.Length > j)
                    {
                        Fitness += 256 - Math.Abs(_console[j] - _trainingResults[i][j]);
                    }
                }

                // Length bonus (percentage of 100).
                countBonus += 200 * ((_trainingResults[i].Length - Math.Abs(_console.Length - _trainingResults[i].Length)) / _trainingResults[i].Length);

                // Make the AI wait until a solution is found without the penalty (too many input characters).
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 20.0);

                Ticks += _bf.m_Ticks;
            }

            if (_fitness != Double.MaxValue)
            {
                _fitness = Fitness + countBonus;
            }

            return _fitness;
        }

        protected override void RunProgramMethod(string program)
        {
            for (int i = 0; i < 99; i++)
            {
                // Get input from the user.
                Console.WriteLine();
                Console.Write(">: ");
                string line = Console.ReadLine();
                int index = 0;

                _console.Clear();

                try
                {
                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        byte b;

                        // Send the next character.
                        if (index < line.Length)
                        {
                            b = (byte)line[index++];
                        }
                        else
                        {
                            b = 0;
                        }

                        return b;
                    },
                    (b) =>
                    {
                        _console.Append((char)b);
                    });

                    bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                Console.WriteLine(_console.ToString());
            }
        }

        public override string GetConstructorParameters()
        {
            return _maxIterationCount.ToString();
        }

        #endregion
    }
}
