using System;

namespace BankBroker.Logic.Models
{
    public class XmlOutputBestExchangeRate : ExchangeRateBase
    {
        public DateTime MinForeignExchangeSellDate { get; set; }

        public DateTime MaxForeignExchangeBuyDate { get; set; }
        
        public float MinForeignExchangeSellValue { get; set; }

        public float MaxForeignExchangeBuyValue { get; set; }
    }
}