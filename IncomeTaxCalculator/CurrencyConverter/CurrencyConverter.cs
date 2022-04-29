using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IncomeTaxCalculator.CurrencyConverter
{
    public static class CurrencyConverter
    {
        /// <summary>
        /// Method <c>convertToPLN</c> converts given currency to it's value in Polish Zloty (PLN).
        /// </summary>
        /// <param name="amount">Gross salary in given currency</param>
        /// <param name="currency">Currency code in 3-letter format as specified in "ISO 4217 Three Letter Currency Codes"</param>
        /// <returns>
        /// Value of given currency in PLN (conversion rates taken from www.exchangerate-api.com)
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when currency code is invalid
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when API key is invalid
        /// </exception>
        public static double convertToPLN(double amount, string currency)
        {
            //API key is stored in App.config for now
            //TODO read about secrets in C#
            string apiKey = ConfigurationManager.AppSettings.Get("ApiExchangeKey");
            //usage of https://www.exchangerate-api.com
            String APIURL = "https://v6.exchangerate-api.com/v6/" + apiKey + "/pair/" + currency + "/PLN";
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    var responseJSON = webClient.DownloadString(APIURL);
                    ExchangeResponseObject response = JsonConvert.DeserializeObject<ExchangeResponseObject>(responseJSON);
                    return response.conversion_rate * amount;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);


                //used for finding error code (403/404/etc) in error message from exchange-api.com
                System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(@"(\d+)(?!.*\d)");
                System.Text.RegularExpressions.MatchCollection match = rx.Matches(ex.Message);

                switch (Int32.Parse(match[0].ToString()))
                {
                    case 403:
                        Console.WriteLine("Wrong API KEY");
                        throw new Exception("Wrong API KEY");
                    case 404:
                        Console.WriteLine("Invalid currency code");
                        throw new ArgumentException("Invalid currency code");

                    default:
                        Console.WriteLine("Unknown error occured when converting currency");
                        throw;
                }

            }

        }
        /// <summary>
        /// Response object containing information received from Exchange-API.com
        /// </summary>
        private class ExchangeResponseObject
        {
            public string result { get; set; }
            public string documentation { get; set; }
            public string terms_of_use { get; set; }
            public string time_last_update_unix { get; set; }
            public string time_last_update_utc { get; set; }
            public string time_next_update_unix { get; set; }
            public string time_next_update_utc { get; set; }
            public string base_code { get; set; }
            public string target_code { get; set; }
            public double conversion_rate { get; set; }
        }
    }
}
