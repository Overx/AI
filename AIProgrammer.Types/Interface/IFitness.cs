﻿namespace AIProgrammer.Types.Interface
{
    public interface IFitness
    {
        /// <summary>
        /// Program source code.
        /// </summary>
        string Program { get; set; }

        /// <summary>
        /// Program output, after running.
        /// </summary>
        string Output { get; set; }
        
        /// <summary>
        /// True fitness. This is the fitness used to determine when a solution is found.
        /// </summary>
        double Fitness { get; set; }

        /// <summary>
        /// Target fitness to achieve a solution.
        /// </summary>
        double TargetFitness { get; }

        /// <summary>
        /// Number of instructions executed for the best fitness.
        /// </summary>
        int Ticks { get; set; }

        /// <summary>
        /// Gets the fitness for the weights. Converts the weights into program code, executes the code, ranks the result.
        /// </summary>
        /// <param name="weights">Array of double</param>
        /// <returns>double</returns>
        double GetFitness(double[] weights);

        /// <summary>
        /// Runs the program source code and returns the output as a string for displaying to the user. Use this with the final result for the user.
        /// </summary>
        /// <param name="program">string</param>
        /// <returns>string (output)</returns>
        string RunProgram(string program);

        /// <summary>
        /// Returns the compilation parameters required to instantiate the fitness constructor (not including GA).
        /// Examples:
        /// AddFitness: _maxIterationCount + ", " + _trainingCount
        /// StringFitness: _maxIterationCount + ", \"" + _targetString + "\""
        /// HelloUserFitness: _maxIterationCount + ", \"" + _targetString + "\", " + _trainingCount
        /// </summary>
        /// <returns></returns>
        string GetConstructorParameters();

        /// <summary>
        /// Resets the target fitness to 0.
        /// </summary>
        void ResetTargetFitness();
    }
}
