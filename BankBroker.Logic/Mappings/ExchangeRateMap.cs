using System.Globalization;
using BankBroker.Logic.Models;
using CsvHelper.Configuration;

namespace BankBroker.Logic.Mappings
{
    public sealed class ExchangeRateMap : CsvClassMap<ExchangeRate>
    {
        public ExchangeRateMap()
        {
            Map(m => m.Country).Index(0);
            Map(m => m.Unit).Index(1);
            Map(m => m.Currency).Index(2);
            Map(m => m.Change).Index(3);
            Map(m => m.ChangeSign).Index(4);
            Map(m => m.ForeignExchangeBuy).Index(5);
            Map(m => m.ForeignExchangeSell).Index(6);
            Map(m => m.ForeignExchangeCenter).Index(7);
            Map(m => m.ForeignCurrencyBuy).Index(8);
            Map(m => m.ForeignCurrencySell).Index(9);
            Map(m => m.ForeignCurrencyCenter).Index(10);
            Map(m => m.CNBExchangeRate).Index(11);
        }
    }
}
