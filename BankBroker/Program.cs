using System;
using System.Xml.Linq;
using BankBroker.Logic;

namespace BankBroker
{
    class Program
    {
        private static DateTime _startDate;
        private static DateTime _endDate;
        private static float _initialCapital;

        private static void Main()
        {
            if (!GetInputData())
            {
                return;
            }

            var fileHelper = new ExchangeRatesFileHelper(_startDate, _endDate, "Downloads");
            var exchangeRatesLoader = new ExchangeRatesLoader(fileHelper);
            var exchangeRatesReader = new ExchangeRatesReader(fileHelper);
            var exchangeRatesAnalyzer = new ExchangeRatesAnalyzer(exchangeRatesLoader, exchangeRatesReader);

            exchangeRatesAnalyzer.Analyze();
            
            var bestExchangeRate = exchangeRatesAnalyzer.FindBestExchangeRate();

            if (bestExchangeRate != null)
            {
                var remainingCapital = exchangeRatesAnalyzer.CalculateRemainingCapital(bestExchangeRate, _initialCapital);

                var resultingXml = new XElement("Root",
                    new XElement("Currency", bestExchangeRate.Currency),
                    new XElement("BestDayOfBuy", bestExchangeRate.MinForeignExchangeSellDate.ToString(fileHelper.DateFormat, fileHelper.CultureInfo)),
                    new XElement("BestDayOfSell", bestExchangeRate.MaxForeignExchangeBuyDate.ToString(fileHelper.DateFormat, fileHelper.CultureInfo)),
                    new XElement("InitialCapital", _initialCapital),
                    new XElement("RemainingCapital", remainingCapital),
                    new XElement("Profit", remainingCapital - _initialCapital));

                resultingXml.Save(fileHelper.ResultingXmlPath);

                Console.WriteLine("Resulting xml file has been generated to Output folder of the current assembly");
            }
            else
            {
                Console.WriteLine("Too short period to make profit :)");
            }

            Console.ReadKey();
        }

        private static bool GetInputData()
        {
            string enteredData;
            
            do
            {
                Console.WriteLine("Please enter start date for calculations: ");

                enteredData = Console.ReadLine();
            } while (!DateTime.TryParse(enteredData, out _startDate));
            
            do
            {
                Console.WriteLine("Please enter end date for calculations: ");

                enteredData = Console.ReadLine();
            } while (!DateTime.TryParse(enteredData, out _endDate));

            do
            {
                Console.WriteLine("Please enter initial capital: ");
                
                enteredData = Console.ReadLine();
            } while (!Single.TryParse(enteredData, out _initialCapital));

            if (_startDate > _endDate || _initialCapital < 0 || _startDate > DateTime.Now || _endDate > DateTime.Now)
            {
                InputDataValidation();

                Console.WriteLine("Please restart the application");
                
                Console.ReadKey();
                
                return false;
            }

            Console.WriteLine();
            Console.WriteLine("Performing calculations...");
            Console.WriteLine();

            return true;
        }

        private static void InputDataValidation()
        {
            if (_startDate > _endDate)
            {
                Console.WriteLine("Start date should be before end date");
            }

            if (_initialCapital < 0)
            {
                Console.WriteLine("Initial capital couldn't be negative");
            }

            if (_startDate > DateTime.Now || _endDate > DateTime.Now)
            {
                Console.WriteLine("Please use date that is less than today's date");
            }
        }
    }
}
