using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class CompositeComponentBuilderTests
	{
		[Fact]
		public void Create_ShouldReturnException_WhenInvalidSfgCompositionIsPassed()
		{
			var mockSfgMappingRepository = new Mock<ICompositeComponentMappingRepository>();
			var mockCounterRepository = new Mock<ICounterGenerator>();
			var mockHeaderColumnDataValidator = new Mock<IHeaderColumnDataValidator>();
			mockHeaderColumnDataValidator.Setup(m => m.Validate(It.IsAny<IEnumerable<IHeaderDefinition>>(), It.IsAny<IEnumerable<IHeaderData>>()))
				.Returns(new Tuple<bool, string>(true, string.Empty));
			var semiFinishedGoodCompositionValidator = new Mock<ICompositeComponentValidator>();
			semiFinishedGoodCompositionValidator.Setup(m => m.Validate("sfg", It.IsAny<IComponentComposition>()))
				.ReturnsAsync(new Tuple<bool, string>(false, "exception occured"));
			var mockSemiFinishedGoodCompositionBuilder = new Mock<ICompositeComponentDataBuilder>();
			var semiFinishedGoodBuilder = new CompositeComponentBuilder(mockSfgMappingRepository.Object,
				mockCounterRepository.Object, mockHeaderColumnDataValidator.Object,
				semiFinishedGoodCompositionValidator.Object, mockSemiFinishedGoodCompositionBuilder.Object);
			var mockSfgDefinition = new Mock<ICompositeComponentDefinition>();
			var mockSfgToBeCreated = new Mock<CompositeComponent>();

			Func<Task> func = async () => await semiFinishedGoodBuilder.Create("sfg", mockSfgDefinition.Object, mockSfgToBeCreated.Object);

			func.ShouldThrow<ArgumentException>().WithMessage("Invalid component composition data.exception occured");
		}

		[Fact]
		public async Task CloneFromService_ShouldUserDefintionTovlidateAndParseDataSent()
		{
			var sfgMappingRepo = new Mock<ICompositeComponentMappingRepository>();
			var mockCounterGenerator = new Mock<ICounterGenerator>();
			var mockheaderColumnValidator = new Mock<IHeaderColumnDataValidator>();

			mockheaderColumnValidator.Setup(
					m => m.Validate(It.IsAny<IEnumerable<IHeaderDefinition>>(), It.IsAny<IEnumerable<IHeaderData>>()))
				.Returns(new Tuple<bool, string>(true, ""));
			var mockSemiFinishedGoodCompositionValidator = new Mock<ICompositeComponentValidator>();
			mockSemiFinishedGoodCompositionValidator.Setup(m => m.Validate("sfg", It.IsAny<IComponentComposition>()))
				.ReturnsAsync(new Tuple<bool, string>(true, string.Empty));
			var mockSemiFinishedGoodCompositionDataBuilder = new Mock<ICompositeComponentDataBuilder>();

			var mockSemiFinishedGoodComposition = new Mock<IComponentComposition>();
			mockSemiFinishedGoodComposition.Setup(m => m.ComponentCoefficients)
				.Returns(new List<ComponentCoefficient>
				{
					new ComponentCoefficient("CCCode", 10, new List<WastagePercentage>
					{
						new WastagePercentage("Wastage", 1)
					}, "UoM", "CCName", ComponentType.Asset),
					new ComponentCoefficient("ServiceCode", 10, new List<WastagePercentage>
					{
						new WastagePercentage("Wastage", 1)
					}, "UoM", "CCName", ComponentType.Service)
				});

			mockSemiFinishedGoodCompositionDataBuilder.Setup(m => m.BuildData(It.IsAny<IComponentComposition>()))
				.ReturnsAsync(mockSemiFinishedGoodComposition.Object);

			CompositeComponentBuilder sut = new CompositeComponentBuilder(sfgMappingRepo.Object,
				mockCounterGenerator.Object,
				mockheaderColumnValidator.Object,
				mockSemiFinishedGoodCompositionValidator.Object,
				mockSemiFinishedGoodCompositionDataBuilder.Object);
			var mockSfgDefinition = new CompositeComponentDefinition();
			mockSfgDefinition.Headers = new List<IHeaderDefinition>();
			mockSfgDefinition.Headers.Add(new HeaderDefinition("", "classification", new List<IColumnDefinition>() { new ColumnDefinition("", "sfg_level_1", new StringDataType()) }));
			mockSfgDefinition.Headers.Add(new HeaderDefinition("", "general", new List<IColumnDefinition>() { new ColumnDefinition("", "sfg_code", new StringDataType()) }));
			Dictionary<string, Dictionary<string, string>> serviceColumnMapping = new Dictionary<string, Dictionary<string, string>>()
			{
				{ "classification", new Dictionary<string, string>(){{"sfg_level_1", "service_level_1"} }}
			};
			Dictionary<string, string> groupCodeMapping = new Dictionary<string, string>()
			{
				{ "Flooring", "FLR" }
			};
			var mockSfgMapping = new CompositeComponentMapping(serviceColumnMapping, groupCodeMapping);
			var service = new Service();

			var headerData = new List<IHeaderData>();
			var classification = new HeaderData("", "classification");
			classification.AddColumns(new ColumnData("", "service_level_1", "Flooring"));
			headerData.Add(classification);
			service.Headers = headerData;
			service.Id = "ServiceCode";
			sfgMappingRepo.Setup(m => m.Get("sfg")).ReturnsAsync(mockSfgMapping);
			mockCounterGenerator.Setup(m => m.Generate("FLR", null, "SFG")).ReturnsAsync("FLR0001");

			var sfg = await sut.CloneFromService(service, mockSfgDefinition,
				mockSemiFinishedGoodComposition.Object);

			sfg.Code = "FLR0001";
			sfg.Group = "Flooring";
			sfg.Headers.Count.Should().Be(2);
			sfg.Headers.Any(
					h => h.Key == "classification" && h.Columns.Any(c => c.Key == "sfg_level_1" && c.Value == "Flooring"))
				.Should()
				.BeTrue();
			sfg.Headers.Any(
				   h => h.Key == "general" && h.Columns.Any(c => c.Key == "sfg_code" && c.Value == "FLR0001"))
			   .Should()
			   .BeTrue();
			sfg.ComponentComposition.Should().NotBeNull();
			sfg.ComponentComposition.ComponentCoefficients.Count().ShouldBeEquivalentTo(2);
			sfg.ComponentComposition.ComponentCoefficients.First().Code.ShouldBeEquivalentTo("CCCode");
			sfg.ComponentComposition.ComponentCoefficients.First().UnitOfMeasure.ShouldBeEquivalentTo("UoM");
			sfg.ComponentComposition.ComponentCoefficients.First().Name.ShouldBeEquivalentTo("CCName");
			sfg.ComponentComposition.ComponentCoefficients.First().Coefficient.ShouldBeEquivalentTo(10);
			sfg.ComponentComposition.ComponentCoefficients.First().WastagePercentages.Count().ShouldBeEquivalentTo(1);
			sfg.ComponentComposition.ComponentCoefficients.First().WastagePercentages.First().Name.ShouldBeEquivalentTo("Wastage");
			sfg.ComponentComposition.ComponentCoefficients.First().WastagePercentages.First().Value.ShouldBeEquivalentTo(1);
		}

		[Fact]
		public void CloneFromService_ShouldThrowArgumentException_WhenFromServiceIsNotPartOfComposition()
		{
			var mockSfgMappingRepository = new Mock<ICompositeComponentMappingRepository>();
			mockSfgMappingRepository.Setup(m => m.Get("sfg"))
				.ReturnsAsync(new CompositeComponentMapping(new Dictionary<string, Dictionary<string, string>>(),
					new Dictionary<string, string>()));
			var mockCounterRepository = new Mock<ICounterGenerator>();
			var mockHeaderColumnDataValidator = new Mock<IHeaderColumnDataValidator>();
			var mockSemiFinishedGoodCompositionValidator = new Mock<ICompositeComponentValidator>();
			var mockSemiFinishedGoodCompositionDataBuilder = new Mock<ICompositeComponentDataBuilder>();

			var semiFinishedGoodBuilder = new CompositeComponentBuilder(mockSfgMappingRepository.Object,
				mockCounterRepository.Object, mockHeaderColumnDataValidator.Object,
				mockSemiFinishedGoodCompositionValidator.Object, mockSemiFinishedGoodCompositionDataBuilder.Object);

			Func<Task> func =
				async () =>
					await semiFinishedGoodBuilder.CloneFromService(new Service() { Id = "code" },
						new CompositeComponentDefinition() { Headers = new List<IHeaderDefinition>() },
						new ComponentComposition(new List<ComponentCoefficient>()));

			func.ShouldThrow<ArgumentException>().WithMessage("fromService code is not part of SFG composition data.");
		}

		[Fact]
		public void Update_ShouldReturnException_WhenInvalidSfgCompositionIsPassed()
		{
			var mockSfgMappingRepository = new Mock<ICompositeComponentMappingRepository>();
			var mockCounterRepository = new Mock<ICounterGenerator>();
			var mockHeaderColumnDataValidator = new Mock<IHeaderColumnDataValidator>();
			mockHeaderColumnDataValidator.Setup(m => m.Validate(It.IsAny<IEnumerable<IHeaderDefinition>>(), It.IsAny<IEnumerable<IHeaderData>>()))
				.Returns(new Tuple<bool, string>(true, string.Empty));
			var semiFinishedGoodCompositionValidator = new Mock<ICompositeComponentValidator>();
			semiFinishedGoodCompositionValidator.Setup(m => m.Validate("sfg", It.IsAny<IComponentComposition>()))
				.ReturnsAsync(new Tuple<bool, string>(false, "exception occured"));
			var mockSemiFinishedGoodCompositionBuilder = new Mock<ICompositeComponentDataBuilder>();
			var semiFinishedGoodBuilder = new CompositeComponentBuilder(mockSfgMappingRepository.Object,
				mockCounterRepository.Object, mockHeaderColumnDataValidator.Object,
				semiFinishedGoodCompositionValidator.Object, mockSemiFinishedGoodCompositionBuilder.Object);
			var mockSfgDefinition = new Mock<ICompositeComponentDefinition>();
			var mockSfgToBeCreated = new Mock<CompositeComponent>();

			Func<Task> func = async () => await semiFinishedGoodBuilder.Update("sfg", mockSfgDefinition.Object, mockSfgToBeCreated.Object);

			func.ShouldThrow<ArgumentException>().WithMessage("Invalid SFG composition data.exception occured");
		}

		[Fact]
		public void Update_ShouldReturnException_WhenInvalidHeaderColumnDataIsPassed()
		{
			var mockSfgMappingRepository = new Mock<ICompositeComponentMappingRepository>();
			var mockCounterRepository = new Mock<ICounterGenerator>();
			var mockHeaderColumnDataValidator = new Mock<IHeaderColumnDataValidator>();
			mockHeaderColumnDataValidator.Setup(m => m.Validate(It.IsAny<IEnumerable<IHeaderDefinition>>(), It.IsAny<IEnumerable<IHeaderData>>()))
				.Returns(new Tuple<bool, string>(false, "exception occured"));
			var semiFinishedGoodCompositionValidator = new Mock<ICompositeComponentValidator>();
			var mockSemiFinishedGoodCompositionBuilder = new Mock<ICompositeComponentDataBuilder>();
			var semiFinishedGoodBuilder = new CompositeComponentBuilder(mockSfgMappingRepository.Object,
				mockCounterRepository.Object, mockHeaderColumnDataValidator.Object,
				semiFinishedGoodCompositionValidator.Object, mockSemiFinishedGoodCompositionBuilder.Object);
			var mockSfgDefinition = new Mock<ICompositeComponentDefinition>();
			var mockSfgToBeCreated = new Mock<CompositeComponent>();

			Func<Task> func = async () => await semiFinishedGoodBuilder.Update("sfg", mockSfgDefinition.Object, mockSfgToBeCreated.Object);

			func.ShouldThrow<ArgumentException>().WithMessage("Invalid header column data.exception occured");
		}

		[Fact]
		public async Task Update_ShouldUpdateSuccessfully_WhenValidSemiFinishedGoodIsPassed()
		{
			var mockSfgMappingRepository = new Mock<ICompositeComponentMappingRepository>();
			var mockCounterRepository = new Mock<ICounterGenerator>();
			var mockHeaderColumnDataValidator = new Mock<IHeaderColumnDataValidator>();
			mockHeaderColumnDataValidator.Setup(m => m.Validate(It.IsAny<IEnumerable<IHeaderDefinition>>(), It.IsAny<IEnumerable<IHeaderData>>()))
				.Returns(new Tuple<bool, string>(true, string.Empty));
			var semiFinishedGoodCompositionValidator = new Mock<ICompositeComponentValidator>();
			semiFinishedGoodCompositionValidator.Setup(m => m.Validate("sfg", It.IsAny<IComponentComposition>()))
				.ReturnsAsync(new Tuple<bool, string>(true, string.Empty));
			var mockSemiFinishedGoodCompositionBuilder = new Mock<ICompositeComponentDataBuilder>();
			mockSemiFinishedGoodCompositionBuilder.Setup(m => m.BuildData(It.IsAny<IComponentComposition>()))
				.ReturnsAsync(new ComponentComposition());
			var semiFinishedGoodBuilder = new CompositeComponentBuilder(mockSfgMappingRepository.Object,
				mockCounterRepository.Object, mockHeaderColumnDataValidator.Object,
				semiFinishedGoodCompositionValidator.Object, mockSemiFinishedGoodCompositionBuilder.Object);
			var sfgDefinition = new CompositeComponentDefinition
			{
				Headers =
					new List<IHeaderDefinition>
					{
						new HeaderDefinition("general", "general",
							new List<IColumnDefinition>
							{
								new ColumnDefinition("sfg_code", "sfg_code", new AutogeneratedDataType("SFG Code"))
							})
					}
			};
			var sfgToBeUpdated = new CompositeComponent
			{
				Headers =
					new List<IHeaderData>
					{
						new HeaderData("general", "general")
						{
							Columns = new List<IColumnData> {new ColumnData("sfg_code", "sfg_code", "sfg_code")}
						}
					}
			};

			var result = await semiFinishedGoodBuilder.Update("sfg", sfgDefinition, sfgToBeUpdated);
			result.Should().NotBeNull();
		}
	}
}