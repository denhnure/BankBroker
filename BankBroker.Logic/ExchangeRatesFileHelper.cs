using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BankBroker.Logic
{
    public class ExchangeRatesFileHelper
    {
        #region Fields

        public readonly string ResultingXmlPath = @"../../Output/Result.xml";
        public readonly string DateFormat = "dd.MM.yyyy";
        public readonly CultureInfo CultureInfo = CultureInfo.InvariantCulture;
        private const string ExchangeRatesUrlPattern = @"http://www.csas.cz/banka/portlets/exchangerates/current.do?csv=1&times=&event=current&day={0}&month={1}&year={2}";
        private readonly string _currencyDataDirectoryPath;
        private readonly DateTime _endDate;
        private DateTime _currentDate;
        private Dictionary<string, string> _urlToFileNameDictionary;

        #endregion

        #region .ctors

        public ExchangeRatesFileHelper(DateTime startDate, DateTime endDate, string currencyDataDirectoryPath)
        {            
            _currentDate = startDate;
            _endDate = endDate;
            _currencyDataDirectoryPath = currencyDataDirectoryPath;
        }

        #endregion

        #region Properties

        public Dictionary<string, string> UrlToFileNameDictionary
        {
            get
            {
                if (_urlToFileNameDictionary == null)
                {
                    _urlToFileNameDictionary = new Dictionary<string, string>();

                    while (_currentDate <= _endDate)
                    {
                        _urlToFileNameDictionary.Add(GenerateExchangeRatesUrl(), GenerateOutputFileName());

                        _currentDate = _currentDate.AddDays(1);
                    }
                }

                return _urlToFileNameDictionary;
            }
        }

        #endregion

        #region Methods

        public void Cleanup()
        {
            if (Directory.Exists(_currencyDataDirectoryPath))
            {
                Directory.Delete(_currencyDataDirectoryPath, true);
            }

            Directory.CreateDirectory(_currencyDataDirectoryPath);
        }

        public DateTime GetDateFromFileName(string fileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            return DateTime.ParseExact(fileNameWithoutExtension, DateFormat, CultureInfo);
        }


        private string GenerateExchangeRatesUrl()
        {
            return string.Format(ExchangeRatesUrlPattern, _currentDate.Day, _currentDate.Month, _currentDate.Year);
        }

        private string GenerateOutputFileName()
        {
            return string.Format("{0}/{1}.csv", _currencyDataDirectoryPath, _currentDate.ToString(DateFormat, CultureInfo));
        }

        #endregion
    }
}