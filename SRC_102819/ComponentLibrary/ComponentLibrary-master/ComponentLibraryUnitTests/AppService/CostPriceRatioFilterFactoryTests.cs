using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
    public class CostPriceRatioFilterFactoryTests
    {
        [Fact]
        public void GetCostPriceRatioFilter_ShouldReturnMaterialCostPriceRatioFilter_WhenMaterialIsPassed()
        {
            var costPriceRatioFilterFactory = new CostPriceRatioFilterFactory();
            var filter = costPriceRatioFilterFactory.GetCostPriceRatioFilter(ComponentType.Material);
            filter.Should().BeOfType<MaterialCostPriceRatioFilter>();
        }

        [Fact]
        public void GetCostPriceRatioFilter_ShouldReturnMaterialCostPriceRatioFilter_WhenServiceIsPassed()
        {
            var costPriceRatioFilterFactory = new CostPriceRatioFilterFactory();
            var filter = costPriceRatioFilterFactory.GetCostPriceRatioFilter(ComponentType.Service);
            filter.Should().BeOfType<CostPriceRatioFilter>();
        }

        [Fact]
        public void GetCostPriceRatioFilter_ShouldReturnMaterialCostPriceRatioFilter_WhenPackageIsPassed()
        {
            var costPriceRatioFilterFactory = new CostPriceRatioFilterFactory();
            var filter = costPriceRatioFilterFactory.GetCostPriceRatioFilter(ComponentType.Package);
            filter.Should().BeOfType<CostPriceRatioFilter>();
        }

        [Fact]
        public void GetCostPriceRatioFilter_ShouldReturnMaterialCostPriceRatioFilter_WhenSfgIsPassed()
        {
            var costPriceRatioFilterFactory = new CostPriceRatioFilterFactory();
            var filter = costPriceRatioFilterFactory.GetCostPriceRatioFilter(ComponentType.SFG);
            filter.Should().BeOfType<CostPriceRatioFilter>();
        }

        [Fact]
        public void GetCostPriceRatioFilter_ShouldReturnMaterialCostPriceRatioFilter_WhenAssetIsPassed()
        {
            var costPriceRatioFilterFactory = new CostPriceRatioFilterFactory();
            Action action = () => costPriceRatioFilterFactory.GetCostPriceRatioFilter(ComponentType.Asset);
            action.ShouldThrow<NotImplementedException>().WithMessage("Asset is not implemented. Try with Material.");

        }
    }
}
