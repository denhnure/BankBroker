using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BankBroker.Logic.Mappings;
using BankBroker.Logic.Models;
using CsvHelper;

namespace BankBroker.Logic
{
    public class ExchangeRatesReader
    {
        #region Fields

        private readonly ExchangeRatesFileHelper _fileHelper;

        #endregion
        
        #region .ctors

        public ExchangeRatesReader(ExchangeRatesFileHelper fileHelper)
        {
            DateToExchangeRateDictionary = new ConcurrentDictionary<DateTime, List<CsvFileExchangeRate>>();
            _fileHelper = fileHelper;
        }

        #endregion

        #region Properties

        public ConcurrentDictionary<DateTime, List<CsvFileExchangeRate>> DateToExchangeRateDictionary { get; private set; }

        #endregion

        #region Methods

        public void ReadCsvFiles()
        {
            Parallel.ForEach(_fileHelper.UrlToFileNameDictionary.Values, ReadCsvFile);
        }

        private void ReadCsvFile(string fileName)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                SkippingTheFirstLine(streamReader);

                using (var csvReader = new CsvReader(streamReader))
                {
                    csvReader.Configuration.RegisterClassMap<ExchangeRateMap>();

                    var csvData = csvReader.GetRecords<CsvFileExchangeRate>().ToList();

                    if (!DateToExchangeRateDictionary.TryAdd(_fileHelper.GetDateFromFileName(fileName), csvData))
                    {
                        throw new Exception("Something strange happenned! Please restart the application!");
                    }
                }
            }
        }

        private static void SkippingTheFirstLine(StreamReader streamReader)
        {
            streamReader.ReadLine();
        }

        #endregion
    }
}