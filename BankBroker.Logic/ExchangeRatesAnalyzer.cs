using System;
using System.Collections.Generic;
using System.Linq;
using BankBroker.Logic.Models;

namespace BankBroker.Logic
{
    public class ExchangeRatesAnalyzer
    {
        #region Fields

        private readonly IExchangeRatesLoader _exchangeRatesLoader;
        private readonly ExchangeRatesReader _exchangeRatesReader;
        private readonly Dictionary<string, XmlOutputBestExchangeRate> _bestExchangeRates; 
        private IOrderedEnumerable<KeyValuePair<DateTime, List<CsvFileExchangeRate>>> _sortedDateToExchangeRateDictionary;

        #endregion

        #region .ctors

        public ExchangeRatesAnalyzer(IExchangeRatesLoader exchangeRatesLoader, ExchangeRatesReader exchangeRatesReader)
        {
            _exchangeRatesLoader = exchangeRatesLoader;
            _exchangeRatesReader = exchangeRatesReader;

            _bestExchangeRates = new Dictionary<string, XmlOutputBestExchangeRate>();
        }

        #endregion

        #region Methods

        public void Analyze()
        {
            _exchangeRatesLoader.DownloadFiles();

            _exchangeRatesReader.ReadCsvFiles();

            _sortedDateToExchangeRateDictionary = _exchangeRatesReader.DateToExchangeRateDictionary.OrderBy(exchangeRate => exchangeRate.Key);

            ProcessItems();

            CalculateProfits();
        }

        public XmlOutputBestExchangeRate FindBestExchangeRate()
        {
            var maxProfit = 0.0f;
            var bestCurrency = String.Empty;
            
            foreach (KeyValuePair<string, XmlOutputBestExchangeRate> exchangeRate in _bestExchangeRates)
            {
                if (exchangeRate.Value.Profit > maxProfit)
                {
                    maxProfit = exchangeRate.Value.Profit;
                    bestCurrency = exchangeRate.Key;
                }
            }

            return string.IsNullOrEmpty(bestCurrency) ? null : _bestExchangeRates[bestCurrency];
        }

        public float CalculateRemainingCapital(XmlOutputBestExchangeRate bestExchangeRate, float initialCapital)
        {
            return ((initialCapital / bestExchangeRate.MinForeignExchangeSellValue) * bestExchangeRate.MaxForeignExchangeBuyValue) / bestExchangeRate.Unit;
        }

        private void ProcessItems()
        {
            foreach (KeyValuePair<DateTime, List<CsvFileExchangeRate>> dateToExchangeRatePair in _sortedDateToExchangeRateDictionary)
            {
                foreach (CsvFileExchangeRate currentCsvFileExchangeRate in dateToExchangeRatePair.Value)
                {
                    ProcessItem(currentCsvFileExchangeRate, dateToExchangeRatePair.Key);
                }
            }
        }

        private void ProcessItem(CsvFileExchangeRate currentCsvFileExchangeRate, DateTime currentDate)
        {
            var currentCurrency = currentCsvFileExchangeRate.Currency;

            if (!_bestExchangeRates.ContainsKey(currentCurrency))
            {
                AddItemToBestExchangeRate(currentCsvFileExchangeRate, currentDate);
            }
            
            if (currentCsvFileExchangeRate.ForeignExchangeSell <= _bestExchangeRates[currentCurrency].MinForeignExchangeSellValue)
            {
                foreach (KeyValuePair<DateTime, List<CsvFileExchangeRate>> dateToExchangeRatePair in _sortedDateToExchangeRateDictionary.SkipWhile(item => currentDate > item.Key))
                {
                    foreach (CsvFileExchangeRate innerLoopCsvFileExchangeRate in dateToExchangeRatePair.Value.Where(item => item.Currency == currentCurrency))
                    {
                        var originDiff = _bestExchangeRates[currentCurrency].MaxForeignExchangeBuyValue - _bestExchangeRates[currentCurrency].MinForeignExchangeSellValue;
                        var newDiff = innerLoopCsvFileExchangeRate.ForeignExchangeBuy - currentCsvFileExchangeRate.ForeignExchangeSell;

                        if (newDiff > originDiff)
                        {
                            _bestExchangeRates[currentCurrency].MinForeignExchangeSellValue = currentCsvFileExchangeRate.ForeignExchangeSell;
                            _bestExchangeRates[currentCurrency].MinForeignExchangeSellDate = currentDate;

                            _bestExchangeRates[currentCurrency].MaxForeignExchangeBuyValue = innerLoopCsvFileExchangeRate.ForeignExchangeBuy;
                            _bestExchangeRates[currentCurrency].MaxForeignExchangeBuyDate = dateToExchangeRatePair.Key;
                        }
                    }
                }
            }
        }

        private void CalculateProfits()
        {
            foreach (KeyValuePair<string, XmlOutputBestExchangeRate> exchangeRate in _bestExchangeRates)
            {
                var currentProfit = exchangeRate.Value.MaxForeignExchangeBuyValue - exchangeRate.Value.MinForeignExchangeSellValue;
                currentProfit /= exchangeRate.Value.Unit;

                exchangeRate.Value.Profit = currentProfit;
            }
        }

        private void AddItemToBestExchangeRate(CsvFileExchangeRate csvFileExchangeRate, DateTime currentDate)
        {
            _bestExchangeRates.Add(csvFileExchangeRate.Currency,
                new XmlOutputBestExchangeRate
                {
                    Country = csvFileExchangeRate.Country,
                    Unit = csvFileExchangeRate.Unit,
                    Currency = csvFileExchangeRate.Currency,
                    MaxForeignExchangeBuyValue = csvFileExchangeRate.ForeignExchangeBuy,
                    MinForeignExchangeSellValue = csvFileExchangeRate.ForeignExchangeSell,
                    MaxForeignExchangeBuyDate = currentDate,
                    MinForeignExchangeSellDate = currentDate
                });
        }

        #endregion
    }
}