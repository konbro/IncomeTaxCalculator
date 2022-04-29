using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncomeTaxCalculator.IncomeTaxCalculator
{
    /// <summary>
    /// Class representing progressive (or regressive depending on tax rates applied) rate tax with 2 tax brackets (one below certain value and other above)
    /// </summary>
    class ProgressiveRateTaxCalculator : ITaxCalculator
    {
        private double _deductibleTax;
        private double _firstTaxBracketTaxRate;
        private double _firstTaxBracketMaxIncome;
        private double _secondTaxBracketTaxRate;

        private double _deductedTax = 0;
        private double _currentNotedIncome = 0;

        /// <summary>
        /// Default constructor with values:
        /// deductible  tax = 525.12
        /// first tax bracket tax rate = 17%
        /// first tax bracket max income = 85528
        /// second tax bracket tax rate = 32%
        /// </summary>
        public ProgressiveRateTaxCalculator()
        {
            _deductibleTax = 525.12;
            _firstTaxBracketTaxRate = 17.0 / 100.0;
            _firstTaxBracketMaxIncome = 85528;
            _secondTaxBracketTaxRate = 32.0 / 100.0;
        }

        /// <summary>
        /// Constructor which allows us to define:
        /// deductible tax
        /// first tax bracket tax
        /// second tax bracket tax
        /// first tax bracket max income
        /// </summary>
        /// <param name="deductibleTax">How much tax deduction we want to be available</param>
        /// <param name="firstTaxBracketTaxRate">Tax rate of first bracket in %</param>
        /// <param name="firstTaxBracketMaxIncome">Max income in first bracket</param>
        /// <param name="secondTaxBracketTaxRate">Tax rate of second bracket in %</param>
        public ProgressiveRateTaxCalculator(double deductibleTax, double firstTaxBracketTaxRate, double firstTaxBracketMaxIncome, double secondTaxBracketTaxRate)
        {
            _deductibleTax = deductibleTax;
            _firstTaxBracketTaxRate = firstTaxBracketTaxRate;
            _firstTaxBracketMaxIncome = firstTaxBracketMaxIncome;
            _secondTaxBracketTaxRate = secondTaxBracketTaxRate;
        }

        /// <summary>
        /// Method which calculates tax for a salary/salaries in multiple currencies (works also for single currency)
        /// </summary>
        /// <param name="listOfSalaries">List of Tuples<double,String> which represent Gross salary and currency code of currrency in which it was received
        /// double - salary
        /// string - Currency code in 3-letter format as specified in "ISO 4217 Three Letter Currency Codes"</param>
        /// <returns>Calculated tax rounded down</returns>
        /// <exception cref="Exception">
        /// Thrown when provided salary is invalid
        /// </exception>
        public double calculateTaxForSalaryInMultipleCurrencies(List<Tuple<double, string>> listOfSalaries)
        {
            double taxDue = 0;

            listOfSalaries.ForEach(delegate (Tuple<double, string> foreignSalary)
            {
                taxDue += calculateTaxForSalaryInOtherCurrencyNotRounded(foreignSalary.Item1, foreignSalary.Item2);
            });
            return Math.Floor(taxDue);
        }

        /// <summary>
        /// Method used for calculating tax for paycheck in currency other than PLN
        /// </summary>
        /// <param name="salary">Gross salary</param>
        /// <param name="currencyCode">Currency code in 3-letter format as specified in "ISO 4217 Three Letter Currency Codes"</param>
        /// <returns>Tax calculated for given salary rounded down</returns>
        /// <exception cref="Exception">
        /// Thrown when provided salary is invalid
        /// </exception>
        public double calculateTaxForSalaryInOtherCurrency(double salary, string currencyCode)
        {
            return Math.Floor(calculateTaxForSalaryInOtherCurrency(salary, currencyCode));
        }
        /// <summary>
        /// Method used for calculating tax for paycheck in currency which requires to know convertion rate
        /// </summary>
        /// <param name="salary">Gross salary</param>
        /// <param name="currencyCode">Currency code in 3-letter format as specified in "ISO 4217 Three Letter Currency Codes"</param>
        /// <returns>Tax in PLN calculated for given salary</returns>
        /// <exception cref="Exception">
        /// Thrown when provided salary is invalid
        /// </exception>
        private double calculateTaxForSalaryInOtherCurrencyNotRounded(double salary, string currencyCode)
        {
            if (salary >= 0)
            {
                //converting PLN to PLN works correctly in currency converter however in order to limit API calls we use our own method
                if (currencyCode.Equals("PLN"))
                {
                    return calculateTaxForSalaryInPLNNotRounded(salary);
                }
                else
                {
                    return calculateTaxForSalaryInPLNNotRounded(CurrencyConverter.CurrencyConverter.convertToPLN(salary, currencyCode));
                }
            }
            else
            {
                throw new Exception("Salary cannot be less than 0");
            }
        }


        /// <summary>
        /// Method calculating tax for given salary.
        /// </summary>
        /// <param name="salary">Gross salary in PLN</param>
        /// <returns>Due tax in PLN rounded down</returns>
        public double calculateTaxForSalaryInPLN(double salary)
        {
            return Math.Floor(calculateTaxForSalaryInPLNNotRounded(salary));
        }

        /// <summary>
        /// Private method used for calculating tax for gross salary in PLN
        /// </summary>
        /// <param name="salary">Gross salary in PLN</param>
        /// <returns>Due tax in PLN</returns>
        /// <exception cref="Exception">
        /// Thrown when provided salary is invalid
        /// </exception>
        private double calculateTaxForSalaryInPLNNotRounded(double salary)
        {
            if (salary >= 0)
            {
                double dueTax = 0;
                if (_currentNotedIncome > _firstTaxBracketMaxIncome)
                {
                    //all of the new income is already in second bracket
                    dueTax = salary * _secondTaxBracketTaxRate;
                }
                else if(_currentNotedIncome + salary < _firstTaxBracketMaxIncome)
                {
                    //all of the income in combination with previous salaries is lower than first bracket limit
                    //therefore it is taxed with tax rate from first bracket
                    dueTax = salary * _firstTaxBracketTaxRate;
                }
                else
                {
                    //case when there is still value in first tax bracket and second tax bracket
                    //calc which part of salary is still taxed with lower rate
                    double salaryInFirstBracket = _firstTaxBracketMaxIncome - _currentNotedIncome;
                    double salaryInSecondBracket = salary - salaryInFirstBracket;
                    dueTax = (salaryInFirstBracket * _firstTaxBracketTaxRate) + (salaryInSecondBracket * _secondTaxBracketTaxRate);
                    
                }
                _currentNotedIncome += salary;
                dueTax = applyTaxDeduction(dueTax);
                return dueTax;
            }
            else
            {
                throw new Exception("Salary cannot be less than 0");
            }
        }

        /// <summary>
        /// Method which applies tax deductions
        /// </summary>
        /// <param name="dueTax"> Tax to which we want to apply our tax deduction</param>
        /// <returns>Tax after applying tax deductions that were left</returns>
        private double applyTaxDeduction(double dueTax)
        {
            double deductibleTaxLeft = _deductibleTax - _deductedTax;
            if (dueTax > deductibleTaxLeft)
            {
                //set _deductedTax to max deductible tax value
                _deductedTax = _deductibleTax;
                //lower the the tax by deductible value left
                dueTax -= deductibleTaxLeft;
            }
            else
            {
                //If tax left is smaller than left deductible tax
                _deductedTax += dueTax;
                dueTax = 0;
            }
            return dueTax;
        }

        /// <summary>
        /// Method which is used to represent new fiscal year 
        /// it sets value of noted income and deducted tax to 0;
        /// </summary>
        public void resetFiscalYear()
        {
            _currentNotedIncome = 0;
            _deductedTax = 0;
        }
    }
}
