namespace BankBroker.Logic.Models
{
    public class CsvFileExchangeRate : ExchangeRateBase
    {
        public float Change { get; set; }

        public string ChangeSign { get; set; }

        public float ForeignExchangeBuy { get; set; }

        public float ForeignExchangeSell { get; set; }

        public float ForeignExchangeCenter { get; set; }

        public string ForeignCurrencyBuy { get; set; }

        public string ForeignCurrencySell { get; set; }

        public string ForeignCurrencyCenter { get; set; }

        public string CNBExchangeRate { get; set; }
    }
}