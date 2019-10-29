using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto
{
    public class ServiceRateDto
    {
        private DateTime _appliedOn;
        private IServiceRate @object;

        public async Task<ServiceRateDto> SetDomain(IServiceRate serviceRate)
        {
            if (serviceRate == null)
                throw new ArgumentException("Service Rate cannot be null.");
            ControlBaseRate = new MoneyDto(serviceRate.ControlBaseRate);
            var landedRate = await serviceRate.LandedRate();
            LandedRate = new MoneyDto(landedRate);
            MarketFluctuation = serviceRate.MarketFluctuation;
            LocationVariance = serviceRate.LocationVariance;
            Location = serviceRate.Location;
            Id = serviceRate.Id;
            AppliedOn = serviceRate.AppliedOn;
            TypeOfPurchase = serviceRate.TypeOfPurchase;
            return this;
        }

        public string TypeOfPurchase { get; set; }

        public virtual IServiceRate Domain(IBank bank, IMasterDataList typeOfPurchaseList, IMasterDataList locationList)
        {
            if (bank == null)
                throw new ArgumentException("Bank cannot be null.");
            if (!typeOfPurchaseList.HasValueIgnoreCase(TypeOfPurchase))
                throw new ArgumentException($"Type of Purchase : {TypeOfPurchase} is invalid.");
            if (!locationList.HasValueIgnoreCase(Location))
                throw new ArgumentException($"Location : {Location} is invalid.");
            return new ServiceRate(
                AppliedOn, Location, Id, ControlBaseRate.Domain(bank), LocationVariance, MarketFluctuation, TypeOfPurchase);
        }

        public string Location { get; set; }

        public string Id { get; set; }

        public virtual DateTime AppliedOn { get; set; }

        public MoneyDto ControlBaseRate { get; set; }

        public MoneyDto LandedRate { get; set; }

        public decimal LocationVariance { get; set; }

        public decimal MarketFluctuation { get; set; }

        protected bool Equals(ServiceRateDto other)
        {
            return string.Equals(TypeOfPurchase, other.TypeOfPurchase) &&
                   string.Equals(Location, other.Location) && string.Equals(Id, other.Id)
                   && DateTime.Equals(AppliedOn, other.AppliedOn) && LocationVariance.Equals(other.LocationVariance)
                   && MarketFluctuation.Equals(other.MarketFluctuation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return (obj.GetType() == GetType()) && Equals((ServiceRateDto)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 21;
                hashCode = (hashCode * 397) ^ TypeOfPurchase.GetHashCode();
                hashCode = (hashCode * 397) ^ (Location?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Id?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ AppliedOn.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(TypeOfPurchase)}: {TypeOfPurchase}, {nameof(Location)}: {Location}, {nameof(Id)}: {Id}, {nameof(AppliedOn)}: {AppliedOn}";
        }
    }
}