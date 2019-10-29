using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class ServiceCostPriceRatioBuilderTests
	{
		[Fact]
		public async Task Create_ShouldPopulateLevels_WhenCodeIsPassed()
		{
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			mockServiceRepository.Setup(m => m.Find(It.IsAny<string>()))
				.ReturnsAsync(
					new Service(
						new List<IHeaderData>
						{
							new HeaderData("Classification", "Classification")
							{
								Columns =
									new List<IColumnData>
									{
										new ColumnData("Service Level 1", "Service Level 1", "FLOORING | DADO | PAVIOUR"),
										new ColumnData("Service Level 2", "Service Level 2", "Flooring")
									}
							}
						}, new ServiceDefinition("FLOORING | DADO | PAVIOUR")));
			ServiceCostPriceRatioBuilder serviceCostPriceRatioBuilder = new ServiceCostPriceRatioBuilder(mockServiceRepository.Object);
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.Service, null, null, null, "FDP0001", null);

			var costPriceRatio = await serviceCostPriceRatioBuilder.Build(costPriceRatioToBeCreated);
			costPriceRatio.Level1.ShouldBeEquivalentTo("FLOORING | DADO | PAVIOUR");
			costPriceRatio.Level2.ShouldBeEquivalentTo("Flooring");
		}

		[Fact]
		public async Task Create_ShouldNotPopulateLevels_WhenCodeIsNotPassed()
		{
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			ServiceCostPriceRatioBuilder serviceCostPriceRatioBuilder = new ServiceCostPriceRatioBuilder(mockServiceRepository.Object);
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.Service, "FLOORING | DADO | PAVIOUR", "Flooring", null, null, null);

			var costPriceRatio = await serviceCostPriceRatioBuilder.Build(costPriceRatioToBeCreated);
			costPriceRatio.Level1.ShouldBeEquivalentTo("FLOORING | DADO | PAVIOUR");
			costPriceRatio.Level2.ShouldBeEquivalentTo("Flooring");
		}
	}
}