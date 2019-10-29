using System;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// The DAO for Material Rate
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity" />
    public class MaterialRateDao : ComponentRateDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRateDao"/> class.
        /// </summary>
        /// <param name="materialRate">The material rate.</param>
        /// <exception cref="System.ArgumentException">Material Rate Entity cannot be null.</exception>
        public MaterialRateDao(IMaterialRate materialRate)
        {
            if (materialRate == null)
                throw new ArgumentException("Material Rate Entity cannot be null.");
            MaterialId = materialRate.Id;
            Location = materialRate.Location;
            AppliedOn = materialRate.AppliedOn;
            TypeOfPurchase = materialRate.TypeOfPurchase;
            InsuranceCharges = materialRate.InsuranceCharges;
            FreightCharges = materialRate.FreightCharges;
            BasicCustomsDuty = materialRate.BasicCustomsDuty;
            ClearanceCharges = materialRate.ClearanceCharges;
            TaxVariance = materialRate.TaxVariance;
            LocationVariance = materialRate.LocationVariance;
            MarketFluctuation = materialRate.MarketFluctuation;
            ControlBaseRate = new MoneyDao(materialRate.ControlBaseRate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRateDao"/> class.
        /// </summary>
        public MaterialRateDao()
        {
        }


        /// <summary>
        /// Gets or sets the market fluctuation.
        /// </summary>
        /// <value>
        /// The market fluctuation.
        /// </value>
        public decimal MarketFluctuation { get; set; }

        /// <summary>
        /// Gets or sets the location variance.
        /// </summary>
        /// <value>
        /// The location variance.
        /// </value>
        public decimal LocationVariance { get; set; }

        /// <summary>
        /// Gets or sets the tax variance.
        /// </summary>
        /// <value>
        /// The tax variance.
        /// </value>
        public decimal TaxVariance { get; set; }

        /// <summary>
        /// Gets or sets the clearance charges.
        /// </summary>
        /// <value>
        /// The clearance charges.
        /// </value>
        public decimal ClearanceCharges { get; set; }

        /// <summary>
        /// Gets or sets the basic customs duty.
        /// </summary>
        /// <value>
        /// The basic customs duty.
        /// </value>
        public decimal BasicCustomsDuty { get; set; }

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
        /// Gets or sets the material identifier.
        /// </summary>
        /// <value>
        /// The material identifier.
        /// </value>
        public virtual string MaterialId { get; set; }

        /// <summary>
        /// Domains the specified bank.
        /// </summary>
        /// <param name="bank">The bank.</param>
        /// <returns></returns>
        public virtual IMaterialRate Domain(IBank bank)
        {
            return new MaterialRate(AppliedOn, Location, MaterialId, ControlBaseRate.Domain(bank), FreightCharges, InsuranceCharges, BasicCustomsDuty, ClearanceCharges, TaxVariance, LocationVariance, MarketFluctuation, TypeOfPurchase);
        }
    }
}