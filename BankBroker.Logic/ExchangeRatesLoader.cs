using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BankBroker.Logic
{
    public class ExchangeRatesLoader : IExchangeRatesLoader
    {
        #region Fields

        private readonly ExchangeRatesFileHelper _fileHelper;

        #endregion

        #region .ctors

        public ExchangeRatesLoader(ExchangeRatesFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        #endregion

        #region Methods

        public void DownloadFiles()
        {
            _fileHelper.Cleanup();

            Parallel.ForEach(_fileHelper.UrlToFileNameDictionary, new ParallelOptions { MaxDegreeOfParallelism = 3 }, DownloadFile);
        }

        private void DownloadFile(KeyValuePair<string,string> exchangeRatePair)
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(exchangeRatePair.Key, exchangeRatePair.Value);
            }
        }

        #endregion
    }
}
