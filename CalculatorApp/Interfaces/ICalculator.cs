using System.ComponentModel;

/*
 * Name: Hieu Dao Le Duc
 * ID: 1924891
 * Assignment 1
 */
namespace CalculatorApp.Interfaces
{
    public interface ICalculator : INotifyPropertyChanged
    {
        string SwitchSignNotation { get; }
        char DecimalPointNotation { get; }
        char PercentNotation { get; }

        char AddNotation { get; }
        char SubstractNotation { get; }
        char MultiplyNotation { get; }
        char DivideNotation { get; }

        string MathExpression { get; }

        void AddToMathExpression(string notation);
        void DeleteOneDigit();
        void Clear();
        bool MathIsComplete();
        void Calculate();
    }
}
