using System;

namespace IncomeTaxCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            
            IncomeTaxCalculator.FlatRateTaxCalculator flatRateTaxCalculaor = new IncomeTaxCalculator.FlatRateTaxCalculator();
            //Console.WriteLine(flatRateTaxCalculaor.calculateTaxForSalaryInOtherCurrency(310.25, "USD"));
            //Console.WriteLine(flatRateTaxCalculaor.calculateTaxForSalaryInOtherCurrency(310.25, "PLN"));
            IncomeTaxCalculator.ProgressiveRateTaxCalculator progressiveRateTaxCalculator = new IncomeTaxCalculator.ProgressiveRateTaxCalculator();
            Console.WriteLine("Tax from 5000 PLN");
            Console.WriteLine(progressiveRateTaxCalculator.calculateTaxForSalaryInPLN(5000.00));
            Console.WriteLine("Tax from 100000 PLN");
            Console.WriteLine(progressiveRateTaxCalculator.calculateTaxForSalaryInPLN(100000.00));
            
            Console.WriteLine("Tax from 100000 PLN");
            Console.WriteLine(progressiveRateTaxCalculator.calculateTaxForSalaryInPLN(100000.00));
            Console.WriteLine("Tax from 100000 PLN");
            Console.WriteLine(progressiveRateTaxCalculator.calculateTaxForSalaryInPLN(100000.00));
            Console.WriteLine("Reset of fiscal year");
            progressiveRateTaxCalculator.resetFiscalYear();
            Console.WriteLine("Tax from 100000 PLN");
            Console.WriteLine(progressiveRateTaxCalculator.calculateTaxForSalaryInPLN(100000.00));

            try {
                Console.WriteLine("Tax from -100000 PLN");
                Console.WriteLine(progressiveRateTaxCalculator.calculateTaxForSalaryInPLN(-100000.00));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Console.ReadLine();
        }
    }
}
