using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncomeTaxCalculator.IncomeTaxCalculator
{
    /// <summary>
    /// Interface used for representation of taxes (in case of this small project: Flat rate tax and progressive tax with 2 brackets)
    /// </summary>
    interface ITaxCalculator
    {
        double calculateTaxForSalaryInPLN(double salary);
        double calculateTaxForSalaryInOtherCurrency(double salary, string currencyCode);
        double calculateTaxForSalaryInMultipleCurrencies(List<Tuple< double, string >> listOfSalaries);
        void resetFiscalYear();
    }
}
