using System;
using BankBroker.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankBroker.Tests
{
    [TestClass]
    public class TestExchangeRatesAnalyzer
    {
        [TestMethod]
        public void TestBestExchangeRate()
        {
            var startDate = new DateTime(2014, 9, 26);
            var endDate = new DateTime(2014, 9, 30);

            var fileHelper = new ExchangeRatesFileHelper(startDate, endDate, "../../TestData");
            var exchangeRatesLoader = new FakeExchangeRatesLoader();
            var exchangeRatesReader = new ExchangeRatesReader(fileHelper);
            var exchangeRatesAnalyzer = new ExchangeRatesAnalyzer(exchangeRatesLoader, exchangeRatesReader);

            exchangeRatesAnalyzer.Analyze();

            var bestExchangeRate = exchangeRatesAnalyzer.FindBestExchangeRate();

            Assert.IsNotNull(bestExchangeRate);
            
            Assert.IsTrue(bestExchangeRate.Currency == "RUB");

            Assert.IsTrue(bestExchangeRate.MinForeignExchangeSellDate == new DateTime(2014, 9, 29));

            Assert.IsTrue(bestExchangeRate.MaxForeignExchangeBuyDate == new DateTime(2014, 9, 30));
        }
    }
}
