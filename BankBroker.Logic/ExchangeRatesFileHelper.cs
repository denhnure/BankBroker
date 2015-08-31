using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BankBroker.Logic
{
    public class ExchangeRatesFileHelper
    {
        #region Fields

        private const string DateFormat = "dd.MM.yyyy";
        private const string ExchangeRatesUrlPattern = @"http://www.csas.cz/banka/portlets/exchangerates/current.do?csv=1&times=&event=current&day={0}&month={1}&year={2}";
        private const string DownloadsDirectoryPath = @"Downloads";
        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;
        private DateTime _currentDate;
        private Dictionary<string, string> _urlToFileNameDictionary;

        #endregion

        #region .ctors

        public ExchangeRatesFileHelper(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            _currentDate = startDate;
            EndDate = endDate;
        }

        #endregion

        #region Properties

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public Dictionary<string, string> UrlToFileNameDictionary
        {
            get
            {
                if (_urlToFileNameDictionary == null)
                {
                    _urlToFileNameDictionary = new Dictionary<string, string>();

                    while (_currentDate <= EndDate)
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
            if (Directory.Exists(DownloadsDirectoryPath))
            {
                Directory.Delete(DownloadsDirectoryPath, true);
            }

            Directory.CreateDirectory(DownloadsDirectoryPath);
        }

        public DateTime GetDateFromFileName(string fileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            return DateTime.ParseExact(fileNameWithoutExtension, DateFormat, _cultureInfo);
        }


        private string GenerateExchangeRatesUrl()
        {
            return string.Format(ExchangeRatesUrlPattern, _currentDate.Day, _currentDate.Month, _currentDate.Year);
        }

        private string GenerateOutputFileName()
        {
            return string.Format("{0}/{1}.csv", DownloadsDirectoryPath, _currentDate.ToString(DateFormat, _cultureInfo));
        }

        #endregion
    }
}