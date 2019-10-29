using System;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto
{
    public class RentalRateDto
    {
        public DateTime AppliedFrom { get; set; }
        public string MaterialId { get; set; }
        public MoneyDto RentalRateValue { get; set; }
        public string UnitOfMeasure { get; set; }

        public RentalRateDto(string materialId, string unitOfMeasure, MoneyDto rentalRateValue, DateTime appliedFrom)
        {
            AppliedFrom = appliedFrom;
            MaterialId = materialId;
            UnitOfMeasure = unitOfMeasure;
            RentalRateValue = rentalRateValue;
        }

        public IRentalRate GetDomain()
        {
            return new RentalRate(MaterialId, UnitOfMeasure, new Money(RentalRateValue.Value, RentalRateValue.Currency), AppliedFrom);
        }
    }
}