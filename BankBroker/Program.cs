using System;
using BankBroker.Logic;

namespace BankBroker
{
    class Program
    {
        private static void Main()
        {
            var startDate = new DateTime(2014, 9, 26);
            var endDate = new DateTime(2014, 12, 30);

            var fileHelper = new ExchangeRatesFileHelper(startDate, endDate);
            var exchangeRatesLoader = new ExchangeRatesLoader(fileHelper);
            var exchangeRatesReader = new ExchangeRatesReader(fileHelper);
            var exchangeRatesAnalyzer = new ExchangeRatesAnalyzer(exchangeRatesLoader, exchangeRatesReader);

            exchangeRatesAnalyzer.Analyze();

            Console.ReadKey();
        }
    }
}
