using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RepositoryPortTests
{
	public class CostPriceRatioRepositoryTests
	{
		[Fact]
		public async Task Create_ShouldAcceptCostPriceRatio_AndInsertIntoMongoCollection()
		{
			Mock<IMongoCollection<CostPriceRatioDao>> mockCostPriceRatioCollection = new Mock<IMongoCollection<CostPriceRatioDao>>();
			CostPriceRatioRepository costPriceRatioRepository = new CostPriceRatioRepository(mockCostPriceRatioCollection.Object);
			CostPriceRatio costPriceRatio = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today,
				ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0001", string.Empty);
			await costPriceRatioRepository.Create(costPriceRatio);
			mockCostPriceRatioCollection.Verify(m => m.InsertOneAsync(It.IsAny<CostPriceRatioDao>(), null, default(CancellationToken)), Times.Once);
		}

		[Fact]
		public void Create_ShouldAcceptCostPriceRatio_AndThrowsDuplicateResourceException()
		{
			Mock<IMongoCollection<CostPriceRatioDao>> mockCostPriceRatioCollection = new Mock<IMongoCollection<CostPriceRatioDao>>();
			mockCostPriceRatioCollection.Setup(
					m => m.CountAsync(It.IsAny<FilterDefinition<CostPriceRatioDao>>(), null, default(CancellationToken)))
				.ReturnsAsync(1);
			CostPriceRatioRepository costPriceRatioRepository = new CostPriceRatioRepository(mockCostPriceRatioCollection.Object);
			CostPriceRatio costPriceRatio = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today,
				ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0001", string.Empty);
			Func<Task> func = async () => await costPriceRatioRepository.Create(costPriceRatio);
			func.ShouldThrow<DuplicateResourceException>();
		}
	}
}