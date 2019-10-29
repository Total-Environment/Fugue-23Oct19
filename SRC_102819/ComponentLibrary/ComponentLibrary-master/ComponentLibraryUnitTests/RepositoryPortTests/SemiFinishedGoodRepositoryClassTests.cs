using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Driver.Core.Operations;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RepositoryPortTests
{
	using Xunit;

	public class SemiFinishedGoodRepositoryClassTests
	{
		[Fact]
		public void Create_ShouldGenerateSearchKeywords_BeforeCreating()
		{
			var mockSemiFinishedGoodCollection = new Mock<IMongoCollection<SemiFinishedGoodDao>>();
			var mockPackageCollection = new Mock<IMongoCollection<PackageDao>>();
			var mockSemiFinishedGoodDefinitionRepository =
				new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			var sut = new CompositeComponentRepository(mockSemiFinishedGoodCollection.Object, mockPackageCollection.Object,
				mockSemiFinishedGoodDefinitionRepository.Object);

			var data = new HeaderColumnData();
			DateTime date = DateTime.UtcNow;
			data.Insert("SomeHeader", CompositeComponentDao.DateCreated, date);
			data.Insert("SomeHeader", CompositeComponentDao.DateLastAmended, date);
			data.Insert("SomeHeader", CompositeComponentDao.CreatedBy, "TE");
			data.Insert("SomeHeader", CompositeComponentDao.LastAmendedBy, "TE");
			data.Insert("SomeHeader", SemiFinishedGoodDao.Code, "FLR003");
			var semiFinishedGood = new CompositeComponent()
			{
				Headers = data.GetData(),
				ComponentComposition = new ComponentComposition(new List<ComponentCoefficient>())
			};

			var sfgDefinition = new CompositeComponentDefinition()
			{
				Headers = new List<IHeaderDefinition>()
				{
					new HeaderDefinition("SomeHeader","SomeHeader",new List<IColumnDefinition>()
					{
						new ColumnDefinition(SemiFinishedGoodDao.Code,SemiFinishedGoodDao.Code,new StringDataType(), true,false)
					})
				}
			};

			var sfg = sut.Create("sfg", semiFinishedGood, sfgDefinition);

			mockSemiFinishedGoodCollection.Verify(m => m.InsertOneAsync(It.Is<SemiFinishedGoodDao>(sfgDao => ((List<string>)sfgDao.Columns["SearchKeywords"])[0] == "FLR003"),
				It.IsAny<InsertOneOptions>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public void Update_ShouldGenerateSearchKeywords_BeforeUpdating()
		{
			var mockCollection = new Mock<IMongoCollection<SemiFinishedGoodDao>>();
			var mockPackageCollection = new Mock<IMongoCollection<PackageDao>>();
			var mockSemiFinishedGoodDefinitionRepository =
				new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();

			var sut = new CompositeComponentRepository(mockCollection.Object, mockPackageCollection.Object,
				mockSemiFinishedGoodDefinitionRepository.Object);

			var data = new HeaderColumnData();
			DateTime date = DateTime.UtcNow;
			data.Insert("SomeHeader", CompositeComponentDao.DateCreated, date);
			data.Insert("SomeHeader", CompositeComponentDao.DateLastAmended, date);
			data.Insert("SomeHeader", CompositeComponentDao.CreatedBy, "TE");
			data.Insert("SomeHeader", CompositeComponentDao.LastAmendedBy, "TE");
			data.Insert("SomeHeader", SemiFinishedGoodDao.Code, "FLR003");
			data.Insert("SomeHeader", SemiFinishedGoodDao.SfgLevel1, "Sfg");
			var semiFinishedGood = new CompositeComponent()
			{
				Headers = data.GetData(),
				ComponentComposition = new ComponentComposition(new List<ComponentCoefficient>())
			};

			TestHelper.MockCollectionWithExisting(mockCollection,
				new SemiFinishedGoodDao(semiFinishedGood, new List<string>()));

			var sfgDefinition = new CompositeComponentDefinition()
			{
				Headers = new List<IHeaderDefinition>()
				{
					new HeaderDefinition("SomeHeader","SomeHeader",new List<IColumnDefinition>()
					{
						new ColumnDefinition(SemiFinishedGoodDao.Code,SemiFinishedGoodDao.Code,new StringDataType(), true,false)
					})
				}
			};

			var sfg = sut.Update("sfg", semiFinishedGood, sfgDefinition);

			mockCollection.Verify(
				m =>
					m.ReplaceOneAsync(It.IsAny<FilterDefinition<SemiFinishedGoodDao>>(),
						It.Is<SemiFinishedGoodDao>(sfgDao => ((List<string>)sfgDao.Columns["SearchKeywords"])[0] == "FLR003"),
						It.IsAny<UpdateOptions>(),
						It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Count_ShouldReturnCorrectCountValue()
		{
			var mockCollection = new Mock<IMongoCollection<SemiFinishedGoodDao>>();
			mockCollection.Setup(
				m =>
					m.CountAsync(It.IsAny<FilterDefinition<SemiFinishedGoodDao>>(), It.IsAny<CountOptions>(),
						It.IsAny<CancellationToken>())).ReturnsAsync(1);
			var mockPackageCollection = new Mock<IMongoCollection<PackageDao>>();
			var mockSemiFinishedGoodDefinitionRepository =
				new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			var semiFinishedGoodRepository = new CompositeComponentRepository(mockCollection.Object, mockPackageCollection.Object,
				mockSemiFinishedGoodDefinitionRepository.Object);

			var result = await semiFinishedGoodRepository.Count("sfg", new Dictionary<string, Tuple<string, object>>());

			result.ShouldBeEquivalentTo(1);
		}

		[Fact]
		public async Task Find_ShouldReturnListOfSemiFinishedGoods()
		{
			var mockCollection = new Mock<IMongoCollection<SemiFinishedGoodDao>>();
			var mockPackageCollection = new Mock<IMongoCollection<PackageDao>>();
			var semiFinishedGood = new CompositeComponent
			{
				Headers =
					new List<IHeaderData>
					{
						new HeaderData("general", "general")
						{
							Columns =
								new List<IColumnData>
								{
									new ColumnData(SemiFinishedGoodDao.Code, SemiFinishedGoodDao.Code, "Code"),
									new ColumnData(SemiFinishedGoodDao.SfgLevel1, SemiFinishedGoodDao.SfgLevel1, "SfgLevel1")
								}
						}
					},
				ComponentComposition = new ComponentComposition(new List<ComponentCoefficient>())
			};
			TestHelper.MockCollectionWithExisting(mockCollection,
				new SemiFinishedGoodDao(
					semiFinishedGood, new List<string>()));
			var mockSemiFinishedGoodDefinitionRepository =
				new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			mockSemiFinishedGoodDefinitionRepository.Setup(m => m.Find("sfg", It.IsAny<string>()))
				.ReturnsAsync(new CompositeComponentDefinition
				{
					Headers =
						new List<IHeaderDefinition>
						{
							new HeaderDefinition("general", "general",
								new List<IColumnDefinition>
								{
									new ColumnDefinition(SemiFinishedGoodDao.Code, SemiFinishedGoodDao.Code, new AutogeneratedDataType("SFG Code")),
									new ColumnDefinition(SemiFinishedGoodDao.SfgLevel1, SemiFinishedGoodDao.SfgLevel1, new StringDataType())
								})
						}
				});
			var semiFinishedGoodRepository = new CompositeComponentRepository(mockCollection.Object, mockPackageCollection.Object,
				mockSemiFinishedGoodDefinitionRepository.Object);

			var result = await semiFinishedGoodRepository.Find("sfg", new Dictionary<string, Tuple<string, object>>(), string.Empty,
				SortOrder.Ascending, 1, 1);

			result.Count.ShouldBeEquivalentTo(1);
		}
	}
}