using System;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Repository.Dao
{
    public class MaterialRateDaoTests
    {
        [Fact]
        public void Equals_ShouldForTheSameValuesInMaterialDao_ReturnTrue()
        {
            var appliedOn = DateTime.Now;
            var materialDao1 = new MaterialRateDao
            {
                Location = "Bangalore",
                TypeOfPurchase = "Import",
                AppliedOn = appliedOn
            };
            var materialDao2 = new MaterialRateDao
            {
                Location = "Bangalore",
                TypeOfPurchase = "Import",
                AppliedOn = appliedOn
            };

            materialDao1.Should().Be(materialDao2);
        }

        [Fact]
        public void Equals_ShouldForDifferentValuesInMaterialDao_ReturnFalse()
        {
            var appliedOn = DateTime.Now;
            var materialDao1 = new MaterialRateDao
            {
                Location = "Bangalore",
                TypeOfPurchase = "Import",
                AppliedOn = appliedOn
            };
            var materialDao2 = new MaterialRateDao
            {
                Location = "Hyderabad",
                TypeOfPurchase = "Import",
                AppliedOn = appliedOn
            };

            materialDao1.Should().NotBe(materialDao2);
        } 
    }
}