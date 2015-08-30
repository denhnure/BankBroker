namespace BankBroker.Logic.Models
{
    public class ExchangeRate
    {
        public string Country { get; set; }

        public int Unit { get; set; }

        public string Currency { get; set; }

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
