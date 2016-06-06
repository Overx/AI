﻿using AIProgrammer.Fitness.Base;
using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Fitness.Concrete
{
    /// <summary>
    /// Calculates the Fibonacci sequence, starting at input1, input2.
    /// Usage:
    /// In App.config:
    /// <add key="BrainfuckVersion" value="2"/>
    /// In Program.cs set:
    /// private static string _appendCode = FibonacciFitness.FibonacciFunctions;
    /// ...
    /// private static IFitness GetFitnessMethod()
    /// {
    ///    return new FibonacciFitness(_ga, _maxIterationCount, 4, 3, _appendCode);
    /// }
    /// </summary>
    public class FibonacciFitness : FitnessBase
    {
        private int _trainingCount;
        private int _maxDigits; // number of fibonacci numbers to calculate.
        private static int _functionCount; // number of functions in the appeneded code.

        /// <summary>
        /// Previously generated BrainPlus function for addition. Generated using AddFitness.
        /// To use, set _appendCode = FibonacciFitness.FibonacciFunctions in main program.
        /// </summary>
        public static string FibonacciFunctions = ",>,-[-<+>]<+.$@";

        public FibonacciFitness(GA ga, int maxIterationCount, int maxDigits = 4, int maxTrainingCount = 3, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            _maxDigits = maxDigits;
            _trainingCount = maxTrainingCount;

            if (_targetFitness == 0)
            {
                _targetFitness = _trainingCount * 256 * _maxDigits;
                _functionCount = CommonManager.GetFunctionCount(appendFunctions);
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            byte input1 = 0, input2 = 0;
            int state = 0;
            double countBonus = 0;
            double penalty = 0;
            List<byte> digits = new List<byte>();

            for (int i = 0; i < _trainingCount; i++)
            {
                switch (i)
                {
                    case 0: input1 = 1; input2 = 2; break;
                    case 1: input1 = 3; input2 = 5; break;
                    case 2: input1 = 8; input2 = 13; break;
                };

                try
                {
                    state = 0;
                    _console.Clear();
                    digits.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state == 0)
                        {
                            state++;
                            return input1;
                        }
                        else if (state == 1)
                        {
                            state++;
                            return input2;
                        }
                        else
                        {
                            // Not ready for input.
                            penalty++;

                            return 0;
                        }
                    },
                    (b) =>
                    {
                        if (state < 2)
                        {
                            // Not ready for output.
                            penalty++;
                        }
                        else
                        {
                            _console.Append(b);
                            _console.Append(",");

                            digits.Add(b);
                        }
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                _output.Append(_console.ToString());
                _output.Append("|");

                // 0,1,1,2,3,5,8,13,21,34,55,89,144,233. Starting at 3 and verifying 10 digits.
                int index = 0;
                int targetValue = input1 + input2; // 1 + 2 = 3
                int lastValue = input2; // 2
                foreach (byte digit in digits)
                {
                    Fitness += 256 - Math.Abs(digit - targetValue);

                    int temp = lastValue; // 2
                    lastValue = targetValue; // 3
                    targetValue += temp; // 3 + 2 = 5

                    if (++index >= _maxDigits)
                        break;
                }

                // Make the AI wait until a solution is found without the penalty (too many input characters).
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 1000.0);

                // Bonus for using functions.
                if (_functionCount > 0)
                {
                    for (char functionName = 'a'; functionName < 'a' + _functionCount; functionName++)
                    {
                        if (MainProgram.Contains(functionName))
                        {
                            countBonus += 25;
                        }
                    }
                }

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
                try
                {
                    int state = 0;

                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        if (state == 0)
                        {
                            state++;
                            Console.WriteLine();
                            Console.Write(">: ");
                            byte b = Byte.Parse(Console.ReadLine());
                            return b;
                        }
                        else if (state == 1)
                        {
                            state++;
                            Console.WriteLine();
                            Console.Write(">: ");
                            byte b = Byte.Parse(Console.ReadLine());
                            return b;
                        }
                        else
                        {
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        Console.Write(b + ",");
                    });

                    bf.Run(_maxIterationCount);
                }
                catch
                {
                }
            }
        }

        public override string GetConstructorParameters()
        {
            return _maxIterationCount + ", " + _maxDigits + ", " + _trainingCount;
        }

        #endregion
    }
}
