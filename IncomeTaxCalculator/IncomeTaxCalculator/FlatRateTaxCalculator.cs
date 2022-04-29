using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncomeTaxCalculator.IncomeTaxCalculator
{

    /// <summary>
    /// Class representing a FLAT RATE TAX. Nothing too complicated, simple checks if paychecks are not negative
    /// Can be also used to represent ExemptFromTax by using constructor with taxRate=0.0
    /// </summary>
    class FlatRateTaxCalculator : ITaxCalculator
    {
        private double _taxRate;

        /// <summary>
        /// Default constructor which creates instance with tax rate of 19%
        /// </summary>
        public FlatRateTaxCalculator()
        {
            _taxRate = 19.0 / 100.0;
        }



        /// <summary>
        /// Constructor in which we can define our flat tax rate. I.e: 0% tax that means that we are exempt from tax.
        /// </summary>
        /// <param name="taxRate">Our rate of a tax in %</param>
        public FlatRateTaxCalculator(double taxRate)
        { 
                _taxRate = taxRate / 100.0;
        }


        /// <summary>
        /// Method used for calculating tax for salary in PLN currency 
        /// </summary>
        /// <param name="salary">Gross salary in PLN</param>
        /// <returns>Tax calculated for given salary rounded down</returns>
        /// <exception cref="Exception">
        /// Thrown when provided salary is invalid
        /// </exception>
        public double calculateTaxForSalaryInPLN(double salary)
        {

            return Math.Floor(calculateTaxForSalaryInPLNNotRounded(salary));
        }

        /// <summary>
        /// Private method used for calculating tax for gross salary in PLN
        /// </summary>
        /// <param name="salary">Gross salary in PLN</param>
        /// <returns>Tax calculated for given salary</returns>
        /// <exception cref="Exception">
        /// Thrown when provided salary is invalid
        /// </exception>
        private double calculateTaxForSalaryInPLNNotRounded(double salary)
        {
            if (salary >= 0)
            {
                return salary * _taxRate;
            }
            else
            {
                throw new Exception("Salary cannot be less than 0");
            }
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

            return Math.Floor(calculateTaxForSalaryInOtherCurrencyNotRounded(salary, currencyCode));
        }

        /// <summary>
        /// Private method used when we want to calculate taxes for paycheck with multiple currencies
        /// </summary>
        /// <param name="salary">Gross salary</param>
        /// <param name="currencyCode">Currency code in 3-letter format as specified in "ISO 4217 Three Letter Currency Codes"</param>
        /// <returns>Tax calculated for given salary</returns>
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
                    return calculateTaxForSalaryInPLN(salary);
                }
                else
                {
                    return CurrencyConverter.CurrencyConverter.convertToPLN(salary, currencyCode) * _taxRate;
                }
            }
            else
            {
                throw new Exception("Salary cannot be less than 0");
            }
        }

        /// <summary>
        /// This method calculates tax for salary given in multiple currencies
        /// </summary>
        /// <param name="listOfSalaries">List of Tuples<double,String> which represent Gross salary and currency code of currrency in which it was received
        /// double - salary
        /// string - Currency code in 3-letter format as specified in "ISO 4217 Three Letter Currency Codes"
        /// </param>
        /// <returns>Tax calculated for given salary rounded down</returns>
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
        /// in case of FlatTaxRate which doesn't have any deductible tax we don't have to reset anything
        /// </summary>
        public void resetFiscalYear()
        {
            //does nothing in this case
        }
    }
}
