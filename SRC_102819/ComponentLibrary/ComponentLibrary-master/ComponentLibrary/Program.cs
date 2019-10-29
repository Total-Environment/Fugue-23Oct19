using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;

namespace ComponentLibrary
{
    public class Program
    {

        public static void Main(string[] args)
        {
            ExchangeRateDto dto =  new ExchangeRateDto()
            {
                AppliedFrom = DateTime.Today,
                BaseConversionRate = "10",
                CurrencyFluctuationCoefficient =  ".4",
                FromCurrency = "USD",
                ToCurrency = "INR"
            };

            Console.WriteLine(dto.ToString());
        }
    }
}