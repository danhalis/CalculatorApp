using CalculatorApp.Enums;
using CalculatorApp.Interfaces;
using CalculatorApp.Models;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

/*
 * Name: Hieu Dao Le Duc
 * ID: 1924891
 * Assignment 1
 */

namespace CalculatorApp
{
    public partial class MainPage : ContentPage
    {
        private ICalculator Calculator { get; set; }
        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

        public MainPage()
        {
            InitializeComponent();

            Calculator = new Calculator
            {
                SubstractNotation = '—',
                MultiplyNotation = 'x',
                DivideNotation = '÷',
                //DecimalPointNotation = ',',
            };

            BindingContext = Calculator;
        }

        private void OnUtilityButtonClicked(object sender, EventArgs e)
        {
            string buttonText = ((Button)sender).Text;

            Calculator.AddToMathExpression(buttonText);
        }

        private void OnSignButtonClicked(object sender, EventArgs e)
        {
            OnUtilityButtonClicked(sender, e);
        }

        private void OnDecimalPointButtonClicked(object sender, EventArgs e)
        {
            OnUtilityButtonClicked(sender, e);
        }

        private void OnNumericButtonClicked(object sender, EventArgs e)
        {
            string buttonText = ((Button)sender).Text;

            Calculator.AddToMathExpression(buttonText);
        }

        private void OnOperatorButtonClicked(object sender, EventArgs e)
        {
            string buttonText = ((Button)sender).Text;

            try
            {
                Calculator.AddToMathExpression(buttonText);
            }
            catch (Exception error)
            {
                _ = DisplayAlert("Mathematical Error", error.Message, "OK");
            }
        }

        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            OnOperatorButtonClicked(sender, e);
        }

        private void OnSubstractButtonClicked(object sender, EventArgs e)
        {
            OnOperatorButtonClicked(sender, e);
        }

        private void OnMultiplyButtonClicked(object sender, EventArgs e)
        {
            OnOperatorButtonClicked(sender, e);
        }

        private void OnDivideButtonClicked(object sender, EventArgs e)
        {
            OnOperatorButtonClicked(sender, e);
        }

        private void OnPercentButtonClicked(object sender, EventArgs e)
        {
            OnUtilityButtonClicked(sender, e);
        }

        private void OnActionButtonClicked(object sender, EventArgs e)
        {
        }

        private void OnClearButtonClicked(object sender, EventArgs e)
        {
            OnActionButtonClicked(sender, e);

            Calculator.Clear();
        }

        private void OnBackspaceButtonClicked(object sender, EventArgs e)
        {
            OnActionButtonClicked(sender, e);

            Calculator.DeleteOneDigit();
        }

        private void OnEqualButtonClicked(object sender, EventArgs e)
        {
            OnActionButtonClicked(sender, e);

            if (Calculator.MathIsComplete())
            {
                try
                {
                    Calculator.Calculate();
                }
                catch (Exception error)
                {
                    _ = DisplayAlert("Mathematical Error", error.Message, "OK");
                }
            }
        }
    }
}
