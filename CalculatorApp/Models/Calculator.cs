using CalculatorApp.Interfaces;
using System;
using System.ComponentModel;
using System.Data;
using System.Text;

/*
 * Name: Hieu Dao Le Duc
 * ID: 1924891
 * Assignment 1
 * 
 *  User Stories:
 * - As a user, I want to enter numbers and operators 
 *   by pressing numeric and operator buttons.
 *   
 * - As a user, I want to enter a decimal number
 *   by pressing "." button:
 *   
 *   + Example: 3 & "." & 5 -> 3.5
 *   + Example: "." & 2 -> 0.2
 *   
 * - As a user, I want to switch the sign of the current new number 
 *   by pressing "+/-" button:
 *   
 *   + After entering a new number then pressing "+/-" button, 
 *   the number should be switched to negative.
 *   + Example: 8 & "+/-" -> -8
 *   + After pressing "+/-" button again, the number should be switched back to positive.
 *   + Example: -8 & "+/-" -> 8
 *   
 * - As a user, I want to quickly enter a percentage
 *   by entering a number (representing the percentage) and pressing "%" button:
 *   
 *   + After entering a number then pressing "%" button, the number should be divided by 100 
 *   and the result should be displayed replacing the initial number.
 *   + Example: 80 & "%" -> 0.8
 *   + After pressing "%" again, the number should be reverted back to its initial value.
 *   This reversion will be discarded if a new number or an operator is entered.
 *   + Example: 80 & "%" -> 0.8 -> "%" -> 80
 *   
 * - As a user, I want to delete one digit from the math expression
 *   by pressing "⌫" button.
 *   
 * - As a user, I want to delete the entire math expression
 *   by pressing "C" button.
 *   
 * - As a user, I want to calculate the math expression that I entered
 *   by pressing "=" button.
 */
namespace CalculatorApp.Models
{
    public class Calculator : ICalculator
    {
        public static string DefaultSwitchSignNotation { get; private set; } = "+/-";
        public static char DefaultDecimalPointNotation { get; private set; } = '.';
        public static char DefaultPercentNotation { get; private set; } = '%';

        public static char DefaultAddNotation { get; private set; } = '+';
        public static char DefaultSubstractNotation { get; private set; } = '-';
        public static char DefaultMultiplyNotation { get; private set; } = '*';
        public static char DefaultDivideNotation { get; private set; } = '/';

        private bool UseCustomizedDecimalPointNotation { get; set; } = false;

        private bool UseCustomizedSubstractNotation { get; set; } = false;
        private bool UseCustomizedMultiplyNotation { get; set; } = false;
        private bool UseCustomizedDivideNotation { get; set; } = false;

        public string SwitchSignNotation { get; } = DefaultSwitchSignNotation;

        private char _decimalPointNotation = DefaultDecimalPointNotation;
        public char DecimalPointNotation
        {
            get => _decimalPointNotation;
            set
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    throw new Exception("The decimal point notation cannot be empty.");
                }

                _decimalPointNotation = value;

                if (_decimalPointNotation != DefaultDecimalPointNotation)
                {
                    UseCustomizedDecimalPointNotation = true;
                }
            }
        }

        public char PercentNotation { get; } = DefaultPercentNotation;

        public char AddNotation { get; } = DefaultAddNotation;

        private char _substractNotation = DefaultSubstractNotation;
        public char SubstractNotation
        {
            get => _substractNotation;
            set
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    throw new Exception("The substract notation cannot be empty.");
                }

                _substractNotation = value;

                if (_substractNotation != DefaultSubstractNotation)
                {
                    UseCustomizedSubstractNotation = true;
                }
            }
        }

        private char _multiplyNotation = DefaultMultiplyNotation;
        public char MultiplyNotation
        {
            get => _multiplyNotation;
            set
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    throw new Exception("The multiply notation cannot be empty.");
                }

                _multiplyNotation = value;

                if (_multiplyNotation != DefaultMultiplyNotation)
                {
                    UseCustomizedMultiplyNotation = true;
                }
            }
        }

        private char _divideNotation = DefaultDivideNotation;
        public char DivideNotation
        {
            get => _divideNotation;
            set
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    throw new Exception("The divide notation cannot be empty.");
                }

                _divideNotation = value;

                if (_divideNotation != DefaultDivideNotation)
                {
                    UseCustomizedDivideNotation = true;
                }
            }
        }

        private StringBuilder MathExpressionBuilder { get; set; } = new StringBuilder("0");

        public string MathExpression => MathExpressionBuilder.ToString();

        private DataTable DataTable { get; set; } = new DataTable();

        private bool AwaitingNewNumber { get; set; } = true;
        private bool NewNumberIsValid { get; set; } = false;

        private bool AllowSwitchSign { get; set; } = true;
        private bool AllowDecimalPoint { get; set; } = true;

        private bool NegativeSignWasApplied { get; set; } = false;
        private bool PercentageWasApplied { get; set; } = false;

        private bool EnteringDivisor { get; set; } = false;

        private int NewNumberStartIndex { get; set; } = 0;
        private int NewNumberLength { get; set; } = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool IsOperator(char notation)
        {
            return notation == AddNotation ||
                notation == SubstractNotation ||
                notation == MultiplyNotation ||
                notation == DivideNotation;
        }

        private string GetNewNumberString()
        {
            return MathExpressionBuilder.ToString(NewNumberStartIndex, NewNumberLength);
        }

        private string Clean(string text)
        {
            string cleanedText = text;
            if (UseCustomizedSubstractNotation)
            {
                cleanedText = cleanedText.Replace(SubstractNotation, DefaultSubstractNotation);
            }
            if (UseCustomizedMultiplyNotation)
            {
                cleanedText = cleanedText.Replace(MultiplyNotation, DefaultMultiplyNotation);
            }
            if (UseCustomizedDivideNotation)
            {
                cleanedText = cleanedText.Replace(DivideNotation, DefaultDivideNotation);
            }
            if (UseCustomizedDecimalPointNotation)
            {
                cleanedText = cleanedText.Replace(DecimalPointNotation, DefaultDecimalPointNotation);
            }
            return cleanedText;
        }

        private string Customize(string text)
        {
            string customizedText = text;
            if (UseCustomizedSubstractNotation)
            {
                customizedText = customizedText.Replace(DefaultSubstractNotation, SubstractNotation);
            }
            if (UseCustomizedMultiplyNotation)
            {
                customizedText = customizedText.Replace(DefaultMultiplyNotation, MultiplyNotation);
            }
            if (UseCustomizedDivideNotation)
            {
                customizedText = customizedText.Replace(DefaultDivideNotation, DivideNotation);
            }
            if (UseCustomizedDecimalPointNotation)
            {
                customizedText = customizedText.Replace(DefaultDecimalPointNotation, DecimalPointNotation);
            }
            return customizedText;
        }

        public bool MathIsComplete()
        {
            return NewNumberIsValid;
        }

        private bool NewNumberIsZeroDivisor()
        {
            return double.Parse(Clean(GetNewNumberString())) == 0;
        }

        public void Calculate()
        {
            if (!MathIsComplete())
            {
                throw new InvalidOperationException("Given math expression is not complete.");
            }

            if (EnteringDivisor)
            {
                if (NewNumberIsZeroDivisor())
                {
                    throw new ArithmeticException("Cannot divide by 0");
                }
            }

            double result = Convert.ToDouble(DataTable.Compute(Clean(MathExpression), ""));

            _ = MathExpressionBuilder.Clear().Append(Customize(result.ToString()));
            ResetBackingFields();
            AwaitingNewNumber = false;
            NewNumberIsValid = true;
            AllowSwitchSign = true;
            ResetNewNumberInfo();
            OnPropertyChanged(nameof(MathExpression));
        }

        public void AddToMathExpression(string notation)
        {
            // If no digit is entered yet
            if (MathExpressionBuilder.Length == 1 && MathExpressionBuilder[0] == '0')
            {

                // If the digit is not a number
                if (!int.TryParse(notation, out _))
                {
                    // If the digit is a decimal point
                    if (notation == DecimalPointNotation.ToString())
                    {
                        AddOneDigitWithoutCheck(notation);
                        AwaitingNewNumber = false;
                        NewNumberIsValid = false;

                        // Update new number info
                        NewNumberLength = 2;
                    }

                    return;
                }

                // If the digit is a number,

                // If the digit to add is '0'
                if (notation == "0")
                {
                    return;
                }

                // Remove '0'
                _ = MathExpressionBuilder.Clear();

                AwaitingNewNumber = false;
                NewNumberIsValid = true;
                AllowSwitchSign = true;

                // Update new number info
                NewNumberLength++;
            }
            // If the digit is a number
            else if (int.TryParse(notation, out _))
            {
                if (!NewNumberIsValid)
                {
                    NewNumberIsValid = true;
                }

                if (AwaitingNewNumber)
                {
                    AwaitingNewNumber = false;

                    AllowSwitchSign = true;

                    // Update new number info
                    NewNumberStartIndex = MathExpressionBuilder.Length;
                    NewNumberLength = 1;
                }
                else
                {
                    // Update new number info
                    NewNumberLength++;
                }

                if (PercentageWasApplied)
                {
                    PercentageWasApplied = false;
                }
            }
            else if (notation == SwitchSignNotation)
            {
                // If there is no number close to yet or if the number is not valid
                if (!AllowSwitchSign)
                {
                    return;
                }

                SwitchSign();
                return;
            }
            // If the notation is percent
            else if (notation == PercentNotation.ToString())
            {
                // If there is no number close to yet or if the number is not valid
                if (!NewNumberIsValid || double.Parse(Clean(GetNewNumberString())) == 0)
                {
                    return;
                }

                ApplyPercentage();
                return;
            }
            // If the notation is decimal point
            else if (notation == DecimalPointNotation.ToString())
            {
                if (!AllowDecimalPoint)
                {
                    return;
                }

                // If awaiting for a new number
                if (AwaitingNewNumber)
                {
                    AwaitingNewNumber = false;

                    AllowSwitchSign = true;

                    // Update new number info
                    NewNumberStartIndex = MathExpressionBuilder.Length;
                    AddOneDigitWithoutCheck("0");
                    NewNumberLength = 2;
                }
                else
                {
                    // Update new number info
                    NewNumberLength++;
                }

                NewNumberIsValid = false;

                AllowDecimalPoint = false;
            }
            // If the notation is an operator
            else if (IsOperator(notation[0]))
            {
                if (!NewNumberIsValid)
                {
                    return;
                }

                if (EnteringDivisor)
                {
                    // If the entered divisor is 0
                    if (NewNumberIsZeroDivisor())
                    {
                        throw new ArithmeticException("Cannot divide by 0");
                    }

                    EnteringDivisor = false;
                }

                if (notation == DivideNotation.ToString())
                {
                    EnteringDivisor = true;
                }

                AwaitingNewNumber = true;
                NewNumberIsValid = false;

                AllowSwitchSign = false;
                AllowDecimalPoint = true;

                NegativeSignWasApplied = false;
                PercentageWasApplied = false;
            }

            // Add the new digit
            AddOneDigitWithoutCheck(notation);
        }

        private void AddOneDigitWithoutCheck(string digit)
        {
            _ = MathExpressionBuilder.Append(digit);
            OnPropertyChanged(nameof(MathExpression));
        }

        private void AddDigitsWithoutCheck(string digits)
        {
            _ = MathExpressionBuilder.Append(digits);
            OnPropertyChanged(nameof(MathExpression));
        }

        private void InsertOneDigitWithoutCheck(int index, string digit)
        {
            _ = MathExpressionBuilder.Insert(index, digit);
            OnPropertyChanged(nameof(MathExpression));
        }

        private void DeleteOneDigitWithoutCheck(int startIndex, int length)
        {
            _ = MathExpressionBuilder.Remove(startIndex, length);
            OnPropertyChanged(nameof(MathExpression));
        }

        private void SwitchSign()
        {
            if (NegativeSignWasApplied)
            {
                DeleteOneDigitWithoutCheck(NewNumberStartIndex, 1);

                // Update new number info
                NewNumberLength--;

                NegativeSignWasApplied = false;
            }
            else
            {
                InsertOneDigitWithoutCheck(NewNumberStartIndex, SubstractNotation.ToString());

                // Update new number info
                NewNumberLength++;

                NegativeSignWasApplied = true;
            }
        }

        private void ApplyPercentage()
        {
            string numberString = GetNewNumberString();

            if (NegativeSignWasApplied)
            {
                numberString = Clean(numberString);
            }

            double number = double.Parse(numberString);
            DeleteDigitsWithoutCheck(NewNumberStartIndex, NewNumberLength);

            string newNumberString;
            if (PercentageWasApplied)
            {
                newNumberString = (number * 100).ToString();
                PercentageWasApplied = false;

            }
            else
            {
                newNumberString = (number / 100).ToString();
                PercentageWasApplied = true;
            }

            if (NegativeSignWasApplied)
            {
                newNumberString = Customize(newNumberString);
            }

            AddDigitsWithoutCheck(newNumberString);

            // Update new number info
            NewNumberLength = newNumberString.Length;
        }

        public void DeleteOneDigit()
        {
            // If no digits is entered yet
            if (MathExpressionBuilder.Length == 1 && MathExpressionBuilder[0] == '0')
            {
                return;
            }

            char digit = MathExpressionBuilder[MathExpressionBuilder.Length - 1];

            // Remove the last digit
            DeleteOneDigitWithoutCheck();

            // If there is no digit left
            if (MathExpressionBuilder.Length == 0)
            {
                // Enter '0' as placeholder
                AddOneDigitWithoutCheck("0");

                ResetBackingFields();

                return;
            }

            // After removing,

            // If the removed digit is a number
            if (int.TryParse(digit.ToString(), out _))
            {
                char currentLastDigit = MathExpressionBuilder[MathExpressionBuilder.Length - 1];

                // If the current last digit is an operator
                if (IsOperator(currentLastDigit))
                {
                    if (NegativeSignWasApplied)
                    {
                        DeleteOneDigitWithoutCheck();
                        NegativeSignWasApplied = false;

                        if (MathExpressionBuilder[MathExpressionBuilder.Length - 1] == DivideNotation)
                        {
                            EnteringDivisor = true;
                        }
                        if (MathExpressionBuilder.Length == 0)
                        {
                            AddOneDigitWithoutCheck("0");
                        }

                        // Update new number info
                        NewNumberLength--;
                    }
                    else
                    {
                        if (currentLastDigit == DivideNotation)
                        {
                            EnteringDivisor = true;
                        }
                    }

                    AwaitingNewNumber = true;
                    NewNumberIsValid = false;

                    AllowSwitchSign = false;
                }
                // If the current last digit is a decimal point
                else if (currentLastDigit == DecimalPointNotation)
                {
                    NewNumberIsValid = false;
                }

                // Update new number info
                NewNumberLength--;
            }
            // If the removed digit is a decimal point
            else if (digit == DecimalPointNotation)
            {
                // if the current last digit is '0'
                if (MathExpressionBuilder[MathExpressionBuilder.Length - 1] == '0')
                {
                    if (MathExpressionBuilder.Length > 1)
                    {
                        DeleteOneDigitWithoutCheck();
                        AwaitingNewNumber = true;
                        NewNumberIsValid = false;
                    }
                    NewNumberLength--;
                }
                else
                {
                    NewNumberIsValid = true;
                }

                AllowDecimalPoint = true;

                // Update new number info
                NewNumberLength--;
            }
            // If the removed digit is an operator
            else if (IsOperator(digit))
            {
                AwaitingNewNumber = false;
                NewNumberIsValid = true;

                AllowSwitchSign = true;

                EnteringDivisor = false;

                ResetNewNumberInfo();
            }

            PercentageWasApplied = false;
        }

        private void ResetNewNumberInfo()
        {
            int i = MathExpressionBuilder.Length - 1;
            NewNumberLength = 0;

            char digit = MathExpressionBuilder[i];
            while (!IsOperator(digit) || i == 0 || IsOperator(MathExpressionBuilder[i - 1]))
            {
                if (digit == DecimalPointNotation)
                {
                    AllowDecimalPoint = false;
                }
                else if (digit == SubstractNotation)
                {
                    NegativeSignWasApplied = true;
                }

                NewNumberLength++;

                i--;
                if (i == -1)
                {
                    break;
                }

                digit = MathExpressionBuilder[i];
            }

            NewNumberStartIndex = i + 1;
        }

        private void DeleteOneDigitWithoutCheck()
        {
            _ = MathExpressionBuilder.Remove(MathExpressionBuilder.Length - 1, 1);
            OnPropertyChanged(nameof(MathExpression));
        }

        private void DeleteDigitsWithoutCheck(int startIndex, int length)
        {
            _ = MathExpressionBuilder.Remove(startIndex, length);
            OnPropertyChanged(nameof(MathExpression));
        }

        public void Clear()
        {
            // Clear everything after the first digit
            // Change the first digit to '0'
            MathExpressionBuilder.Remove(1, MathExpressionBuilder.Length - 1)[0] = '0';

            ResetBackingFields();

            OnPropertyChanged(nameof(MathExpression));
        }

        private void ResetBackingFields()
        {
            AwaitingNewNumber = true;
            NewNumberIsValid = false;

            AllowDecimalPoint = true;
            AllowSwitchSign = false;

            NegativeSignWasApplied = false;
            PercentageWasApplied = false;

            EnteringDivisor = false;

            NewNumberStartIndex = 0;
            NewNumberLength = 0;
        }
    }
}
