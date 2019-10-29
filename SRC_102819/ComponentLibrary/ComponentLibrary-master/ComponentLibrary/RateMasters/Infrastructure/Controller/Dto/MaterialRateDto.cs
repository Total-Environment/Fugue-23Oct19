using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// The DTO for Material Rates
    /// </summary>
    public class MaterialRateDto
    {
        public async Task<MaterialRateDto> SetDomain(IMaterialRate materialRate)
        {
            if (materialRate == null)
                throw new ArgumentException("Material Rate cannot be null.");
            FreightCharges = materialRate.FreightCharges;
            InsuranceCharges = materialRate.InsuranceCharges;
            BasicCustomsDuty = materialRate.BasicCustomsDuty;
            ClearanceCharges = materialRate.ClearanceCharges;
            TaxVariance = materialRate.TaxVariance;
            LocationVariance = materialRate.LocationVariance;
            MarketFluctuation = materialRate.MarketFluctuation;
            Location = materialRate.Location;
            Id = materialRate.Id;
            AppliedOn = materialRate.AppliedOn;
            TypeOfPurchase = materialRate.TypeOfPurchase;
            var landedRate = await materialRate.LandedRate();
            LandedRate = new MoneyDto(landedRate);
            ControlBaseRate = new MoneyDto(materialRate.ControlBaseRate);
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRateDto"/> class.
        /// </summary>
        public MaterialRateDto()
        {
        }


        /// <summary>
        /// Gets or sets the insurance charges.
        /// </summary>
        /// <value>
        /// The insurance charges.
        /// </value>
        public decimal InsuranceCharges { get; set; }

        /// <summary>
        /// Gets or sets the freight charges.
        /// </summary>
        /// <value>
        /// The freight charges.
        /// </value>
        public decimal FreightCharges { get; set; }

        /// <summary>
        /// Gets or sets the basic customs duty.
        /// </summary>
        /// <value>
        /// The basic customs duty.
        /// </value>
        public decimal BasicCustomsDuty { get; set; }

        /// <summary>
        /// Gets or sets the clearance charges.
        /// </summary>
        /// <value>
        /// The clearance charges.
        /// </value>
        public decimal ClearanceCharges { get; set; }

        /// <summary>
        /// Gets or sets the tax variance.
        /// </summary>
        /// <value>
        /// The tax variance.
        /// </value>
        public decimal TaxVariance { get; set; }

        /// <summary>
        /// Gets or sets the location variance.
        /// </summary>
        /// <value>
        /// The location variance.
        /// </value>
        public decimal LocationVariance { get; set; }

        /// <summary>
        /// Gets or sets the market fluctuation.
        /// </summary>
        /// <value>
        /// The market fluctuation.
        /// </value>
        public decimal MarketFluctuation { get; set; }

        /// <summary>
        /// Gets or sets the type of purchase.
        /// </summary>
        /// <value>
        /// The type of purchase.
        /// </value>
        public string TypeOfPurchase { get; set; }

        /// <summary>
        /// Gets or sets the control base rate.
        /// </summary>
        /// <value>
        /// The control base rate.
        /// </value>
        public MoneyDto ControlBaseRate { get; set; }


        /// <summary>
        /// Gets the landed rate.
        /// </summary>
        /// <value>
        /// The landed rate.
        /// </value>
        public MoneyDto LandedRate { get; set; }

        /// <summary>
        /// Domains the specified bank.
        /// </summary>
        /// <param name="bank">The bank.</param>
        /// <param name="typeOfPurchaseList">The type of purchase.</param>
        /// <param name="locationList">The location.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Bank cannot be null.</exception>
        public virtual IMaterialRate Domain(IBank bank, IMasterDataList typeOfPurchaseList, IMasterDataList locationList)
        {
            if (bank == null)
                throw new ArgumentException("Bank cannot be null.");
            if (!typeOfPurchaseList.HasValueIgnoreCase(TypeOfPurchase))
                throw new ArgumentException($"Type of Purchase : {TypeOfPurchase} is invalid.");
            if (!locationList.HasValueIgnoreCase(Location))
                throw new ArgumentException($"Location : {Location} is invalid.");
            return new MaterialRate(
                AppliedOn,
                Location,
                Id,
                ControlBaseRate.Domain(bank),
                FreightCharges,
                InsuranceCharges,
                BasicCustomsDuty,
                ClearanceCharges,
                TaxVariance,
                LocationVariance,
                MarketFluctuation,
                TypeOfPurchase
            );
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the applied on.
        /// </summary>
        /// <value>
        /// The applied on.
        /// </value>
        public virtual DateTime AppliedOn { get; set; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(MaterialRateDto other)
        {
            return string.Equals(TypeOfPurchase, other.TypeOfPurchase) &&
                   string.Equals(Location, other.Location) && string.Equals(Id, other.Id)
                   && DateTime.Equals(AppliedOn, other.AppliedOn) && InsuranceCharges.Equals(other.InsuranceCharges) &&
                   FreightCharges.Equals(other.FreightCharges) && BasicCustomsDuty.Equals(other.BasicCustomsDuty) &&
                   ClearanceCharges.Equals(other.ClearanceCharges) && TaxVariance.Equals(other.TaxVariance) &&
                   LocationVariance.Equals(other.LocationVariance) && MarketFluctuation.Equals(other.MarketFluctuation);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return (obj.GetType() == GetType()) && Equals((MaterialRateDto)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"{nameof(TypeOfPurchase)}: {TypeOfPurchase}, {nameof(Location)}: {Location}, {nameof(Id)}: {Id}, {nameof(AppliedOn)}: {AppliedOn}";
        }
    }
}